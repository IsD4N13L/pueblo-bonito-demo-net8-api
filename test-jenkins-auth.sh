#!/bin/bash

# =============================================================================
# Script de Prueba de Autenticación Jenkins
# =============================================================================
# Este script te ayuda a probar la conectividad y autenticación con Jenkins
# desde tu servidor local o desde GitHub Actions
# =============================================================================

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Función para logging
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Función para mostrar ayuda
show_help() {
    echo "Uso: $0 [opciones]"
    echo ""
    echo "Opciones:"
    echo "  -h, --help              Mostrar esta ayuda"
    echo "  -u, --user USER         Usuario de Jenkins"
    echo "  -t, --token TOKEN       Token de Jenkins"
    echo "  -s, --server SERVER     URL del servidor Jenkins (ej: http://192.168.1.100:8080)"
    echo "  -j, --job JOB           Nombre del job a probar (opcional)"
    echo "  -v, --verbose           Modo verbose"
    echo ""
    echo "Variables de entorno:"
    echo "  JENKINS_USER           Usuario de Jenkins"
    echo "  JENKINS_TOKEN          Token de Jenkins" 
    echo "  JENKINS_URL            URL del servidor Jenkins"
    echo "  JENKINS_JOB            Nombre del job (opcional)"
    echo ""
    echo "Ejemplos:"
    echo "  $0 -u admin -t 11234567890abcdef -s http://192.168.1.100:8080"
    echo "  $0 -u admin -t 11234567890abcdef -s http://192.168.1.100:8080 -j my-pipeline"
    echo ""
}

# Variables por defecto
JENKINS_USER=${JENKINS_USER:-""}
JENKINS_TOKEN=${JENKINS_TOKEN:-""}
JENKINS_URL=${JENKINS_URL:-""}
JENKINS_JOB=${JENKINS_JOB:-""}
VERBOSE=false

# Parsear argumentos
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        -u|--user)
            JENKINS_USER="$2"
            shift 2
            ;;
        -t|--token)
            JENKINS_TOKEN="$2"
            shift 2
            ;;
        -s|--server)
            JENKINS_URL="$2"
            shift 2
            ;;
        -j|--job)
            JENKINS_JOB="$2"
            shift 2
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        *)
            log_error "Opción desconocida: $1"
            show_help
            exit 1
            ;;
    esac
done

# Validar parámetros requeridos
if [[ -z "$JENKINS_USER" ]]; then
    log_error "Usuario de Jenkins no especificado. Use -u o variable JENKINS_USER"
    exit 1
fi

if [[ -z "$JENKINS_TOKEN" ]]; then
    log_error "Token de Jenkins no especificado. Use -t o variable JENKINS_TOKEN"
    exit 1
fi

if [[ -z "$JENKINS_URL" ]]; then
    log_error "URL de Jenkins no especificada. Use -s o variable JENKINS_URL"
    exit 1
fi

# Limpiar URL (remover slash final)
JENKINS_URL=$(echo "$JENKINS_URL" | sed 's/\/$//')

log_info "=== PRUEBA DE AUTENTICACIÓN JENKINS ==="
log_info "Usuario: $JENKINS_USER"
log_info "Servidor: $JENKINS_URL"
log_info "Token: ${JENKINS_TOKEN:0:8}..." # Solo mostrar primeros 8 caracteres
echo ""

# Test 1: Conectividad básica
log_info "🔍 Test 1: Verificando conectividad básica..."
if curl -s --connect-timeout 10 --max-time 20 "$JENKINS_URL" > /dev/null 2>&1; then
    log_success "✅ Servidor Jenkins accesible"
else
    log_error "❌ No se puede conectar al servidor Jenkins"
    log_error "   Verifica que:"
    log_error "   - El servidor esté ejecutándose"
    log_error "   - La URL sea correcta"
    log_error "   - No haya problemas de red/firewall"
    exit 1
fi

# Test 2: API básica sin autenticación
log_info "🔍 Test 2: Verificando API de Jenkins..."
API_RESPONSE=$(curl -s -w "%{http_code}" -o /tmp/jenkins_api_test.json "$JENKINS_URL/api/json")
if [[ "$API_RESPONSE" == "200" ]]; then
    log_success "✅ API de Jenkins responde correctamente"
elif [[ "$API_RESPONSE" == "403" ]]; then
    log_warning "⚠️  API requiere autenticación (esperado)"
else
    log_warning "⚠️  API responde con código: $API_RESPONSE"
fi

# Test 3: Autenticación
log_info "🔍 Test 3: Probando autenticación..."
AUTH_RESPONSE=$(curl -s -w "%{http_code}" \
    --user "$JENKINS_USER:$JENKINS_TOKEN" \
    -o /tmp/jenkins_auth_test.json \
    "$JENKINS_URL/api/json")

if [[ "$AUTH_RESPONSE" == "200" ]]; then
    log_success "✅ Autenticación exitosa"
    
    # Extraer información del usuario
    if command -v jq >/dev/null 2>&1; then
        JENKINS_VERSION=$(jq -r '.version // "N/A"' /tmp/jenkins_auth_test.json 2>/dev/null || echo "N/A")
        log_info "   Versión de Jenkins: $JENKINS_VERSION"
    fi
elif [[ "$AUTH_RESPONSE" == "401" ]]; then
    log_error "❌ Error de autenticación (401 Unauthorized)"
    log_error "   Posibles causas:"
    log_error "   - Usuario incorrecto"
    log_error "   - Token incorrecto o expirado"
    log_error "   - Token no tiene permisos suficientes"
    exit 1
elif [[ "$AUTH_RESPONSE" == "403" ]]; then
    log_error "❌ Acceso denegado (403 Forbidden)"
    log_error "   El usuario existe pero no tiene permisos para acceder a la API"
    exit 1
else
    log_error "❌ Error de autenticación (código: $AUTH_RESPONSE)"
    if [[ "$VERBOSE" == "true" ]]; then
        log_error "Respuesta del servidor:"
        cat /tmp/jenkins_auth_test.json 2>/dev/null || echo "Sin respuesta"
    fi
    exit 1
fi

# Test 4: Listar jobs disponibles
log_info "🔍 Test 4: Listando jobs disponibles..."
JOBS_RESPONSE=$(curl -s -w "%{http_code}" \
    --user "$JENKINS_USER:$JENKINS_TOKEN" \
    -o /tmp/jenkins_jobs.json \
    "$JENKINS_URL/api/json?tree=jobs[name,url,buildable]")

if [[ "$JOBS_RESPONSE" == "200" ]]; then
    if command -v jq >/dev/null 2>&1; then
        JOBS_COUNT=$(jq '.jobs | length' /tmp/jenkins_jobs.json 2>/dev/null || echo "0")
        log_success "✅ Se encontraron $JOBS_COUNT jobs"
        
        if [[ "$JOBS_COUNT" -gt 0 ]]; then
            log_info "Jobs disponibles:"
            jq -r '.jobs[] | "   - \(.name) (buildable: \(.buildable))"' /tmp/jenkins_jobs.json 2>/dev/null || true
        fi
    else
        log_success "✅ Lista de jobs obtenida (instala jq para ver detalles)"
    fi
else
    log_warning "⚠️  No se pudo obtener lista de jobs (código: $JOBS_RESPONSE)"
fi

# Test 5: Probar job específico (si se especificó)
if [[ -n "$JENKINS_JOB" ]]; then
    log_info "🔍 Test 5: Verificando job específico '$JENKINS_JOB'..."
    
    JOB_RESPONSE=$(curl -s -w "%{http_code}" \
        --user "$JENKINS_USER:$JENKINS_TOKEN" \
        -o /tmp/jenkins_job_test.json \
        "$JENKINS_URL/job/$JENKINS_JOB/api/json")
    
    if [[ "$JOB_RESPONSE" == "200" ]]; then
        log_success "✅ Job '$JENKINS_JOB' existe y es accesible"
        
        if command -v jq >/dev/null 2>&1; then
            BUILDABLE=$(jq -r '.buildable // false' /tmp/jenkins_job_test.json 2>/dev/null)
            LAST_BUILD=$(jq -r '.lastBuild.number // "N/A"' /tmp/jenkins_job_test.json 2>/dev/null)
            log_info "   Buildable: $BUILDABLE"
            log_info "   Último build: #$LAST_BUILD"
        fi
        
        # Test 5.1: Probar trigger del job (DRY RUN)
        log_info "🔍 Test 5.1: Probando acceso a trigger del job..."
        TRIGGER_URL="$JENKINS_URL/job/$JENKINS_JOB/build"
        
        # Solo hacer HEAD request para no disparar el job
        TRIGGER_RESPONSE=$(curl -s -I -w "%{http_code}" \
            --user "$JENKINS_USER:$JENKINS_TOKEN" \
            "$TRIGGER_URL" 2>/dev/null | tail -n1)
        
        if [[ "$TRIGGER_RESPONSE" == "405" ]]; then
            log_success "✅ Endpoint de trigger accesible (405 Method Not Allowed es esperado para HEAD)"
        elif [[ "$TRIGGER_RESPONSE" == "200" ]] || [[ "$TRIGGER_RESPONSE" == "201" ]]; then
            log_success "✅ Endpoint de trigger accesible"
        elif [[ "$TRIGGER_RESPONSE" == "403" ]]; then
            log_error "❌ Sin permisos para disparar el job"
        else
            log_warning "⚠️  Respuesta inesperada del trigger: $TRIGGER_RESPONSE"
        fi
        
    elif [[ "$JOB_RESPONSE" == "404" ]]; then
        log_error "❌ Job '$JENKINS_JOB' no existe"
    elif [[ "$JOB_RESPONSE" == "403" ]]; then
        log_error "❌ Sin permisos para acceder al job '$JENKINS_JOB'"
    else
        log_error "❌ Error al acceder al job (código: $JOB_RESPONSE)"
    fi
fi

# Test 6: Verificar permisos del usuario
log_info "🔍 Test 6: Verificando permisos del usuario..."
USER_RESPONSE=$(curl -s -w "%{http_code}" \
    --user "$JENKINS_USER:$JENKINS_TOKEN" \
    -o /tmp/jenkins_user.json \
    "$JENKINS_URL/user/$JENKINS_USER/api/json")

if [[ "$USER_RESPONSE" == "200" ]]; then
    log_success "✅ Información del usuario accesible"
    
    if command -v jq >/dev/null 2>&1; then
        FULL_NAME=$(jq -r '.fullName // "N/A"' /tmp/jenkins_user.json 2>/dev/null)
        log_info "   Nombre completo: $FULL_NAME"
    fi
else
    log_warning "⚠️  No se pudo obtener información del usuario (código: $USER_RESPONSE)"
fi

# Limpiar archivos temporales
rm -f /tmp/jenkins_*.json

echo ""
log_success "🎉 PRUEBAS COMPLETADAS"
log_info "Si todos los tests pasaron, tu configuración de Jenkins está correcta."
log_info "Si hay errores, revisa los mensajes anteriores para diagnosticar el problema."
echo ""
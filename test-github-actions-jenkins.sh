#!/bin/bash

# =============================================================================
# Script de Prueba para GitHub Actions + Jenkins
# =============================================================================
# Este script simula exactamente lo que hace GitHub Actions para conectarse
# con Jenkins y ayuda a diagnosticar problemas de autenticación
# =============================================================================

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
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

log_step() {
    echo -e "${PURPLE}[STEP]${NC} $1"
}

# Función para mostrar ayuda
show_help() {
    echo "Script de prueba para GitHub Actions + Jenkins"
    echo ""
    echo "Este script simula exactamente los pasos que realiza GitHub Actions"
    echo "para conectarse con Jenkins y ejecutar jobs."
    echo ""
    echo "Variables de entorno requeridas:"
    echo "  JENKINS_INTERNAL_IP    IP interna del servidor Jenkins"
    echo "  JENKINS_PORT           Puerto de Jenkins (default: 8080)"
    echo "  JENKINS_USER           Usuario de Jenkins"
    echo "  JENKINS_TOKEN          Token de Jenkins"
    echo ""
    echo "Variables opcionales:"
    echo "  JENKINS_JOB            Nombre del job a probar (default: test-job)"
    echo "  GITHUB_SHA             SHA del commit (simulado)"
    echo "  GITHUB_REF             Referencia de la rama (simulado)"
    echo "  GITHUB_RUN_NUMBER      Número de ejecución (simulado)"
    echo ""
    echo "Ejemplo:"
    echo "  export JENKINS_INTERNAL_IP=192.168.1.100"
    echo "  export JENKINS_USER=admin"
    echo "  export JENKINS_TOKEN=11234567890abcdef"
    echo "  $0"
    echo ""
}

# Verificar si se solicita ayuda
if [[ "$1" == "-h" ]] || [[ "$1" == "--help" ]]; then
    show_help
    exit 0
fi

# Verificar variables requeridas
if [[ -z "$JENKINS_INTERNAL_IP" ]]; then
    log_error "Variable JENKINS_INTERNAL_IP no está definida"
    show_help
    exit 1
fi

if [[ -z "$JENKINS_USER" ]]; then
    log_error "Variable JENKINS_USER no está definida"
    show_help
    exit 1
fi

if [[ -z "$JENKINS_TOKEN" ]]; then
    log_error "Variable JENKINS_TOKEN no está definida"
    show_help
    exit 1
fi

# Variables por defecto
JENKINS_PORT=${JENKINS_PORT:-8080}
JENKINS_JOB=${JENKINS_JOB:-"test-job"}
GITHUB_SHA=${GITHUB_SHA:-$(date +%s | sha256sum | cut -c1-8)}
GITHUB_REF=${GITHUB_REF:-"refs/heads/main"}
GITHUB_RUN_NUMBER=${GITHUB_RUN_NUMBER:-42}

# Construir URL de Jenkins
JENKINS_URL="http://$JENKINS_INTERNAL_IP:$JENKINS_PORT"

# Información del build simulado
BUILD_NUMBER=$GITHUB_RUN_NUMBER
COMMIT_SHA=${GITHUB_SHA:0:8}
BRANCH=${GITHUB_REF#refs/heads/}

log_step "🚀 SIMULANDO GITHUB ACTIONS + JENKINS"
echo "================================"
log_info "Jenkins IP: $JENKINS_INTERNAL_IP"
log_info "Jenkins URL: $JENKINS_URL"
log_info "Usuario: $JENKINS_USER"
log_info "Token: ${JENKINS_TOKEN:0:8}..."
log_info "Job: $JENKINS_JOB"
log_info "Build: #$BUILD_NUMBER"
log_info "Branch: $BRANCH"
log_info "Commit: $COMMIT_SHA"
echo "================================"
echo ""

# PASO 1: Verificar conectividad (como en GitHub Actions)
log_step "PASO 1: 🔍 Test Jenkins Connectivity"
log_info "Verificando conectividad a Jenkins..."

# Ping al servidor Jenkins
log_info "Haciendo ping al servidor..."
if ping -c 3 -W 3 $JENKINS_INTERNAL_IP >/dev/null 2>&1; then
    log_success "✅ Ping a Jenkins exitoso"
else
    log_warning "⚠️ Ping a Jenkins falló, pero continuando..."
fi

# Verificar acceso HTTP a Jenkins
log_info "Verificando acceso HTTP..."
RETRY_COUNT=0
JENKINS_ACCESSIBLE=false

while [ $RETRY_COUNT -lt 5 ]; do
    if curl -s --connect-timeout 10 --max-time 20 "$JENKINS_URL/api/json" >/dev/null 2>&1; then
        log_success "✅ Jenkins accesible en: $JENKINS_URL"
        JENKINS_ACCESSIBLE=true
        break
    fi
    
    log_info "⏳ Esperando acceso a Jenkins... ($((RETRY_COUNT + 1))/5)"
    sleep 5
    RETRY_COUNT=$((RETRY_COUNT + 1))
done

if [[ "$JENKINS_ACCESSIBLE" == "false" ]]; then
    log_error "❌ No se pudo acceder a Jenkins después de 5 intentos"
    exit 1
fi

echo ""

# PASO 2: Ejecutar Job de Jenkins (simulando GitHub Actions)
log_step "PASO 2: ⚙️ Trigger Jenkins Job"
log_info "Ejecutando job de Jenkins..."

JOB_NAME="$JENKINS_JOB"

# Parámetros del job (exactamente como en GitHub Actions)
BUILD_PARAMS="BUILD_NUMBER=$BUILD_NUMBER"
BUILD_PARAMS="${BUILD_PARAMS}&COMMIT_SHA=$COMMIT_SHA"
BUILD_PARAMS="${BUILD_PARAMS}&BRANCH=$BRANCH"
BUILD_PARAMS="${BUILD_PARAMS}&ENVIRONMENT=staging"
BUILD_PARAMS="${BUILD_PARAMS}&GITHUB_RUN_ID=$GITHUB_RUN_NUMBER"
BUILD_PARAMS="${BUILD_PARAMS}&TRIGGERED_BY=GitHub Actions Test"

log_info "🎯 Job: $JOB_NAME"
log_info "📋 Parámetros: $BUILD_PARAMS"

# Ejecutar job con parámetros (exactamente como en GitHub Actions)
JENKINS_JOB_URL="${JENKINS_URL}/job/${JOB_NAME}/buildWithParameters"

log_info "🚀 Disparando job de Jenkins..."
log_info "URL: $JENKINS_JOB_URL"

# Crear archivo temporal para la respuesta
RESPONSE_FILE="/tmp/jenkins_response_$(date +%s).txt"

HTTP_CODE=$(curl -s -w "%{http_code}" \
    --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
    --data "$BUILD_PARAMS" \
    --output "$RESPONSE_FILE" \
    "$JENKINS_JOB_URL")

log_info "📊 Código de respuesta HTTP: $HTTP_CODE"

if [ "$HTTP_CODE" -eq 201 ] || [ "$HTTP_CODE" -eq 200 ]; then
    log_success "✅ Job de Jenkins disparado correctamente"
    
    # Obtener número de build de la queue
    log_info "🔍 Obteniendo información del build..."
    QUEUE_INFO=$(curl -s \
        --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
        "${JENKINS_URL}/job/${JOB_NAME}/api/json" 2>/dev/null || echo '{}')
    
    if command -v jq >/dev/null 2>&1; then
        NEXT_BUILD=$(echo "$QUEUE_INFO" | jq -r '.nextBuildNumber // empty')
        if [ ! -z "$NEXT_BUILD" ] && [ "$NEXT_BUILD" != "null" ]; then
            ESTIMATED_BUILD=$((NEXT_BUILD - 1))
            log_info "🔢 Número de build estimado: $ESTIMATED_BUILD"
        fi
    fi
    
elif [ "$HTTP_CODE" -eq 401 ]; then
    log_error "❌ Error de autenticación (401 Unauthorized)"
    log_error "Causas posibles:"
    log_error "  - Usuario incorrecto: '$JENKINS_USER'"
    log_error "  - Token incorrecto o expirado"
    log_error "  - Token sin permisos suficientes"
    echo ""
    log_error "SOLUCIONES:"
    log_error "1. Verificar que el usuario existe en Jenkins"
    log_error "2. Regenerar el token en Jenkins:"
    log_error "   - Ir a Jenkins > Usuario > Configurar > API Token"
    log_error "   - Crear nuevo token y actualizar secrets de GitHub"
    log_error "3. Verificar que el token tiene permisos para:"
    log_error "   - Leer jobs"
    log_error "   - Ejecutar builds"
    
elif [ "$HTTP_CODE" -eq 403 ]; then
    log_error "❌ Acceso denegado (403 Forbidden)"
    log_error "El usuario existe pero no tiene permisos suficientes"
    echo ""
    log_error "SOLUCIONES:"
    log_error "1. Verificar permisos del usuario en Jenkins:"
    log_error "   - Manage Jenkins > Manage Users > $JENKINS_USER"
    log_error "2. Asignar permisos necesarios:"
    log_error "   - Job/Build"
    log_error "   - Job/Read"
    log_error "   - Overall/Read"
    
elif [ "$HTTP_CODE" -eq 404 ]; then
    log_error "❌ Job no encontrado (404 Not Found)"
    log_error "El job '$JOB_NAME' no existe en Jenkins"
    echo ""
    log_error "SOLUCIONES:"
    log_error "1. Verificar que el job existe en Jenkins"
    log_error "2. Crear el job si no existe"
    log_error "3. Verificar el nombre del job en el workflow"
    
else
    log_error "❌ Error al disparar job de Jenkins (código: $HTTP_CODE)"
    log_error "📋 Respuesta del servidor:"
    cat "$RESPONSE_FILE" 2>/dev/null || echo "Sin respuesta"
fi

# Limpiar archivo temporal
rm -f "$RESPONSE_FILE"

echo ""

# PASO 3: Verificar información del job (si existe)
log_step "PASO 3: 📊 Verificar Job Information"

JOB_INFO_URL="${JENKINS_URL}/job/${JOB_NAME}/api/json"
log_info "Obteniendo información del job..."

JOB_INFO_RESPONSE=$(curl -s -w "%{http_code}" \
    --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
    -o "/tmp/job_info.json" \
    "$JOB_INFO_URL")

if [ "$JOB_INFO_RESPONSE" -eq 200 ]; then
    log_success "✅ Información del job obtenida"
    
    if command -v jq >/dev/null 2>&1; then
        BUILDABLE=$(jq -r '.buildable // false' /tmp/job_info.json)
        LAST_BUILD=$(jq -r '.lastBuild.number // "N/A"' /tmp/job_info.json)
        LAST_SUCCESS=$(jq -r '.lastSuccessfulBuild.number // "N/A"' /tmp/job_info.json)
        
        log_info "   Buildable: $BUILDABLE"
        log_info "   Último build: #$LAST_BUILD"
        log_info "   Último exitoso: #$LAST_SUCCESS"
    fi
    
elif [ "$JOB_INFO_RESPONSE" -eq 404 ]; then
    log_error "❌ Job '$JOB_NAME' no encontrado"
    
    # Listar jobs disponibles
    log_info "📋 Listando jobs disponibles..."
    JOBS_RESPONSE=$(curl -s \
        --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
        "${JENKINS_URL}/api/json?tree=jobs[name]")
    
    if command -v jq >/dev/null 2>&1; then
        AVAILABLE_JOBS=$(echo "$JOBS_RESPONSE" | jq -r '.jobs[].name' 2>/dev/null || echo "No se pudo obtener lista")
        if [[ -n "$AVAILABLE_JOBS" ]]; then
            log_info "Jobs disponibles:"
            echo "$AVAILABLE_JOBS" | while read -r job; do
                log_info "   - $job"
            done
        fi
    fi
    
else
    log_warning "⚠️ No se pudo obtener información del job (código: $JOB_INFO_RESPONSE)"
fi

# Limpiar archivos temporales
rm -f /tmp/job_info.json

echo ""

# PASO 4: Diagnóstico adicional
log_step "PASO 4: 🔧 Diagnóstico Adicional"

# Verificar versión de Jenkins
log_info "Verificando versión de Jenkins..."
VERSION_RESPONSE=$(curl -s \
    --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
    "${JENKINS_URL}/api/json")

if command -v jq >/dev/null 2>&1; then
    JENKINS_VERSION=$(echo "$VERSION_RESPONSE" | jq -r '.version // "N/A"')
    log_info "Versión de Jenkins: $JENKINS_VERSION"
fi

# Verificar plugins necesarios
log_info "Verificando plugins instalados..."
PLUGINS_RESPONSE=$(curl -s \
    --user "${JENKINS_USER}:${JENKINS_TOKEN}" \
    "${JENKINS_URL}/pluginManager/api/json?depth=1")

if command -v jq >/dev/null 2>&1; then
    PLUGIN_COUNT=$(echo "$PLUGINS_RESPONSE" | jq '.plugins | length' 2>/dev/null || echo "0")
    log_info "Plugins instalados: $PLUGIN_COUNT"
fi

echo ""

# RESUMEN FINAL
log_step "📋 RESUMEN DE DIAGNÓSTICO"
echo "================================"

if [ "$HTTP_CODE" -eq 201 ] || [ "$HTTP_CODE" -eq 200 ]; then
    log_success "✅ CONFIGURACIÓN CORRECTA"
    log_info "Tu configuración de GitHub Actions + Jenkins está funcionando correctamente."
else
    log_error "❌ CONFIGURACIÓN CON PROBLEMAS"
    log_info "Revisa los errores anteriores y aplica las soluciones sugeridas."
fi

echo ""
log_info "Para usar este diagnóstico en GitHub Actions:"
log_info "1. Verifica que los secrets estén configurados correctamente:"
log_info "   - JENKINS_INTERNAL_IP"
log_info "   - JENKINS_USER"
log_info "   - JENKINS_TOKEN"
log_info "   - JENKINS_PORT (opcional, default: 8080)"
log_info ""
log_info "2. Asegúrate de que el job especificado existe en Jenkins"
log_info "3. Verifica los permisos del usuario en Jenkins"

echo "================================"
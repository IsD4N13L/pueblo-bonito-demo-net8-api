# 🔧 Guía de Troubleshooting: GitHub Actions + Jenkins + OpenVPN

Esta guía te ayuda a diagnosticar y resolver problemas de autenticación entre GitHub Actions y Jenkins a través de OpenVPN.

## 📋 Tabla de Contenidos

1. [Diagnóstico Rápido](#diagnóstico-rápido)
2. [Problemas Comunes](#problemas-comunes)
3. [Verificación de Configuración](#verificación-de-configuración)
4. [Scripts de Prueba](#scripts-de-prueba)
5. [Soluciones Paso a Paso](#soluciones-paso-a-paso)

## 🚀 Diagnóstico Rápido

### Usar los Scripts de Prueba

```bash
# 1. Hacer ejecutables los scripts
chmod +x test-jenkins-auth.sh test-github-actions-jenkins.sh

# 2. Probar autenticación básica (desde tu servidor local)
./test-jenkins-auth.sh -u tu_usuario -t tu_token -s http://192.168.1.100:8080

# 3. Simular GitHub Actions (desde tu servidor)
export JENKINS_INTERNAL_IP=192.168.1.100
export JENKINS_USER=tu_usuario
export JENKINS_TOKEN=tu_token
export JENKINS_JOB=nombre_del_job
./test-github-actions-jenkins.sh
```

## 🔍 Problemas Comunes

### 1. Error 401 - Unauthorized

**Síntomas:**
- GitHub Actions falla con código HTTP 401
- Mensaje: "❌ Error de autenticación (401 Unauthorized)"

**Causas Posibles:**
- Usuario incorrecto
- Token incorrecto o expirado
- Token sin permisos suficientes

**Soluciones:**

#### A) Verificar Usuario
```bash
# En Jenkins, ir a:
# Manage Jenkins > Manage Users
# Verificar que el usuario existe y está activo
```

#### B) Regenerar Token
1. En Jenkins: `Usuario > Configurar > API Token`
2. Revocar token existente
3. Generar nuevo token
4. Actualizar secret `JENKINS_TOKEN` en GitHub

#### C) Verificar Permisos del Token
```bash
# Probar con curl desde tu servidor
curl -u "usuario:token" http://tu-jenkins:8080/api/json
```

### 2. Error 403 - Forbidden

**Síntomas:**
- Autenticación funciona pero no puede ejecutar jobs
- Código HTTP 403

**Causas:**
- Usuario sin permisos suficientes
- Configuración de seguridad restrictiva

**Soluciones:**

#### A) Asignar Permisos Necesarios
En Jenkins: `Manage Jenkins > Configure Global Security > Authorization`

Permisos mínimos requeridos:
- `Overall/Read`
- `Job/Read`
- `Job/Build`
- `Job/Workspace` (opcional)

#### B) Verificar Matrix-based Security
```bash
# Si usas Matrix-based security, asegúrate de que el usuario tenga:
- Overall: Read
- Job: Build, Read, Workspace
```

### 3. Error 404 - Job No Encontrado

**Síntomas:**
- Error: "Job 'nombre' no existe"
- Código HTTP 404

**Soluciones:**

#### A) Verificar Nombre del Job
```bash
# Listar jobs disponibles
curl -u "usuario:token" "http://jenkins:8080/api/json?tree=jobs[name]" | jq '.jobs[].name'
```

#### B) Actualizar Workflow
En `.github/workflows/jenkins-pipeline.yml`, líneas 52-61:
```yaml
# Verificar que los nombres coincidan con Jenkins
if [ "${{ github.ref }}" == "refs/heads/main" ]; then
  echo "jenkins_job=tu-job-real" >> $GITHUB_OUTPUT
```

### 4. Problemas de Conectividad VPN

**Síntomas:**
- VPN no se conecta
- No se puede acceder a Jenkins después de VPN

**Soluciones:**

#### A) Verificar Configuración OpenVPN
```bash
# El secret OPENVPN_CLIENT_CONF debe contener:
client
dev tun
proto udp
remote tu-servidor-vpn 1194
resolv-retry infinite
nobind
persist-key
persist-tun
ca ca.crt
cert client.crt
key client.key
verb 3
```

#### B) Verificar IP Interna
```bash
# Después de conectar VPN, verificar que la IP sea accesible
ping $JENKINS_INTERNAL_IP
telnet $JENKINS_INTERNAL_IP 8080
```

## ⚙️ Verificación de Configuración

### GitHub Secrets Requeridos

| Secret | Descripción | Ejemplo |
|--------|-------------|---------|
| `OPENVPN_CLIENT_CONF` | Configuración completa del cliente OpenVPN | `client\ndev tun\n...` |
| `JENKINS_INTERNAL_IP` | IP privada del servidor Jenkins | `192.168.1.100` |
| `JENKINS_PORT` | Puerto de Jenkins (opcional) | `8080` |
| `JENKINS_USER` | Usuario de Jenkins | `admin` |
| `JENKINS_TOKEN` | API Token de Jenkins | `11234567890abcdef` |

### Verificar Secrets en GitHub

1. Ve a tu repositorio en GitHub
2. `Settings > Secrets and variables > Actions`
3. Verifica que todos los secrets estén configurados
4. Los valores no deben tener espacios extra ni caracteres especiales

### Configuración de Jenkins

#### 1. Habilitar API REST
```bash
# Manage Jenkins > Configure Global Security
# Marcar: "Enable security"
# Permitir acceso a API sin CSRF (para scripts)
```

#### 2. Crear Usuario para GitHub Actions
```bash
# Manage Jenkins > Manage Users > Create User
# Usuario: github-actions (o el que prefieras)
# Asignar permisos necesarios
```

#### 3. Generar API Token
```bash
# Usuario > Configure > API Token > Add new Token
# Guardar el token generado (solo se muestra una vez)
```

## 🧪 Scripts de Prueba

### Script 1: test-jenkins-auth.sh
Prueba autenticación básica con Jenkins

```bash
./test-jenkins-auth.sh -u admin -t token -s http://192.168.1.100:8080 -j mi-job
```

### Script 2: test-github-actions-jenkins.sh
Simula exactamente lo que hace GitHub Actions

```bash
export JENKINS_INTERNAL_IP=192.168.1.100
export JENKINS_USER=admin
export JENKINS_TOKEN=token
./test-github-actions-jenkins.sh
```

## 🛠️ Soluciones Paso a Paso

### Solución 1: Configurar Usuario y Token desde Cero

#### Paso 1: Crear Usuario en Jenkins
```bash
1. Ir a Jenkins > Manage Jenkins > Manage Users
2. Click "Create User"
3. Llenar datos:
   - Username: github-actions
   - Password: (temporal, no se usará)
   - Full name: GitHub Actions User
   - Email: github-actions@tu-empresa.com
```

#### Paso 2: Asignar Permisos
```bash
1. Manage Jenkins > Configure Global Security
2. Authorization: "Matrix-based security"
3. Agregar usuario "github-actions"
4. Marcar permisos:
   ✅ Overall: Read
   ✅ Job: Build, Read, Workspace
```

#### Paso 3: Generar Token
```bash
1. Click en usuario "github-actions" (esquina superior derecha)
2. Configure
3. API Token > Add new Token
4. Name: "GitHub Actions Token"
5. Generate > Copiar token
```

#### Paso 4: Actualizar GitHub Secrets
```bash
1. GitHub repo > Settings > Secrets and variables > Actions
2. Actualizar JENKINS_USER = "github-actions"
3. Actualizar JENKINS_TOKEN = "token-copiado"
```

### Solución 2: Crear Job de Prueba

#### Crear Job Simple
```groovy
// En Jenkins: New Item > Pipeline > OK
// Pipeline script:
pipeline {
    agent any
    parameters {
        string(name: 'BUILD_NUMBER', defaultValue: '1', description: 'Build number from GitHub')
        string(name: 'COMMIT_SHA', defaultValue: '', description: 'Commit SHA')
        string(name: 'BRANCH', defaultValue: 'main', description: 'Branch name')
    }
    stages {
        stage('Test') {
            steps {
                echo "Build #${params.BUILD_NUMBER}"
                echo "Commit: ${params.COMMIT_SHA}"
                echo "Branch: ${params.BRANCH}"
                echo "✅ GitHub Actions integration working!"
            }
        }
    }
}
```

### Solución 3: Debug del Workflow

#### Agregar Debug al Workflow
```yaml
# En .github/workflows/jenkins-pipeline.yml
# Agregar antes del step "Trigger Jenkins Job":

- name: 🐛 Debug Information
  run: |
    echo "=== DEBUG INFORMATION ==="
    echo "JENKINS_INTERNAL_IP: ${{ secrets.JENKINS_INTERNAL_IP }}"
    echo "JENKINS_USER: ${{ secrets.JENKINS_USER }}"
    echo "JENKINS_TOKEN: ${JENKINS_TOKEN:0:8}..." # Solo primeros 8 caracteres
    echo "JENKINS_URL: http://${{ secrets.JENKINS_INTERNAL_IP }}:${{ secrets.JENKINS_PORT || '8080' }}"
    echo "=========================="
  env:
    JENKINS_TOKEN: ${{ secrets.JENKINS_TOKEN }}
```

## 📊 Códigos de Error y Significados

| Código | Significado | Acción |
|--------|-------------|--------|
| 200/201 | ✅ Éxito | Todo funciona correctamente |
| 401 | ❌ No autorizado | Verificar usuario/token |
| 403 | ❌ Prohibido | Verificar permisos |
| 404 | ❌ No encontrado | Verificar nombre del job |
| 500 | ❌ Error interno | Revisar logs de Jenkins |

## 🔧 Comandos Útiles para Debug

### Desde tu Servidor (con acceso directo a Jenkins)
```bash
# Probar conectividad
curl -I http://localhost:8080

# Probar autenticación
curl -u "usuario:token" http://localhost:8080/api/json

# Listar jobs
curl -u "usuario:token" http://localhost:8080/api/json?tree=jobs[name] | jq '.jobs[].name'

# Disparar job manualmente
curl -X POST -u "usuario:token" "http://localhost:8080/job/mi-job/build"
```

### Desde GitHub Actions (agregar al workflow)
```bash
# En un step de debug
- name: Test Jenkins Connection
  run: |
    curl -u "${{ secrets.JENKINS_USER }}:${{ secrets.JENKINS_TOKEN }}" \
         "http://${{ secrets.JENKINS_INTERNAL_IP }}:8080/api/json"
```

## 📞 Checklist Final

Antes de ejecutar el workflow, verifica:

- [ ] OpenVPN configurado correctamente
- [ ] Jenkins accesible desde la red VPN
- [ ] Usuario de Jenkins existe y está activo
- [ ] Token de Jenkins generado y válido
- [ ] Permisos del usuario configurados
- [ ] Job existe en Jenkins
- [ ] Todos los secrets configurados en GitHub
- [ ] Scripts de prueba ejecutados exitosamente

## 🆘 Si Nada Funciona

1. **Ejecutar scripts de prueba localmente** primero
2. **Revisar logs de Jenkins** en `/var/log/jenkins/jenkins.log`
3. **Verificar logs de OpenVPN** en GitHub Actions
4. **Crear un job de prueba simple** en Jenkins
5. **Usar workflow manual** para hacer debug paso a paso

---

💡 **Tip:** Siempre prueba la configuración localmente antes de usar GitHub Actions. Los scripts proporcionados te ayudarán a identificar problemas rápidamente.
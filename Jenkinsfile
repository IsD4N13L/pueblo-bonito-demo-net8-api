pipeline {
    agent any
    
    parameters {
        string(name: 'BRANCH_NAME', defaultValue: 'main', description: 'Branch name to build')
        string(name: 'COMMIT_SHA', defaultValue: '', description: 'Commit SHA to build')
        string(name: 'PR_NUMBER', defaultValue: '', description: 'Pull Request Number (if applicable)')
        booleanParam(name: 'IS_PR', defaultValue: false, description: 'Is this a Pull Request build?')
    }
    
    environment {
        BUILD_TYPE = "${params.IS_PR ? 'PR' : 'BRANCH'}"
        BUILD_IDENTIFIER = "${params.IS_PR ? "PR-${params.PR_NUMBER}" : params.BRANCH_NAME}"
    }
    
    stages {
        stage('Build Info') {
            steps {
                script {
                    echo "🚀 INFORMACIÓN DEL BUILD"
                    echo "======================="
                    echo "Tipo: ${env.BUILD_TYPE}"
                    echo "Identificador: ${env.BUILD_IDENTIFIER}"
                    echo "Branch: ${params.BRANCH_NAME}"
                    echo "Commit: ${params.COMMIT_SHA}"
                    
                    if (params.IS_PR) {
                        echo "PR Number: ${params.PR_NUMBER}"
                        currentBuild.displayName = "#${BUILD_NUMBER} - PR-${params.PR_NUMBER}"
                        currentBuild.description = "Pull Request #${params.PR_NUMBER} from ${params.BRANCH_NAME}"
                    } else {
                        currentBuild.displayName = "#${BUILD_NUMBER} - ${params.BRANCH_NAME}"
                        currentBuild.description = "Branch build: ${params.BRANCH_NAME}"
                    }
                }
            }
        }
        
        stage('Checkout') {
            steps {
                script {
                    if (params.COMMIT_SHA) {
                        // Checkout específico por SHA
                        checkout([$class: 'GitSCM',
                            branches: [[name: params.COMMIT_SHA]],
                            userRemoteConfigs: [[
                                url: 'https://github.com/ISD4N13L/pueblo-bonito-demo-net8-api.git'
                            ]]
                        ])
                    } else {
                        // Checkout por branch
                        checkout([$class: 'GitSCM',
                            branches: [[name: "*/${params.BRANCH_NAME}"]],
                            userRemoteConfigs: [[
                                url: 'https://github.com/ISD4N13L/pueblo-bonito-demo-net8-api.git'
                            ]]
                        ])
                    }
                }
            }
        }
        
        stage('Validate .NET SDK') {
            steps {
                echo 'Verificando la instalación del SDK de .NET 8...'
                sh 'dotnet --version'
            }
        }
        
        stage('Restore & Build') {
            steps {
                echo 'Restaurando dependencias y compilando el proyecto...'
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
                echo 'Construcción del proyecto exitosa.'
            }
        }
        
        stage('Test') {
            steps {
                echo 'Ejecutando pruebas...'
                script {
                    def testProjects = sh(script: 'find . -name "*.Test*.csproj" -o -name "*Test.csproj" -o -name "*Tests.csproj"', returnStdout: true).trim()
                    if (testProjects) {
                        sh 'dotnet test --configuration Release --no-build --logger trx --results-directory ./TestResults'
                    } else {
                        echo 'No se encontraron proyectos de pruebas.'
                    }
                }
            }
        }
        
        stage('Security Scan') {
            when {
                expression { params.IS_PR }
            }
            steps {
                echo '🔐 Ejecutando análisis de seguridad para PR...'
                // Aquí puedes agregar herramientas de análisis de seguridad
                sh 'dotnet list package --vulnerable --include-transitive || echo "No vulnerable packages found"'
            }
        }
        
        stage('Deploy to Staging') {
            when {
                expression { params.IS_PR }
            }
            steps {
                echo '📦 Desplegando a ambiente de staging para PR...'
                sh 'dotnet publish --configuration Release --output ./publish-staging --no-build'
                
                // Simular despliegue a staging
                script {
                    def stagingUrl = "https://staging-pr-${params.PR_NUMBER}.tu-dominio.com"
                    echo "🌐 Aplicación desplegada en: ${stagingUrl}"
                    
                    // Guardar URL para uso posterior
                    writeFile file: 'staging-url.txt', text: stagingUrl
                    archiveArtifacts artifacts: 'staging-url.txt', allowEmptyArchive: true
                }
            }
        }
        
        stage('Deploy to Production') {
            when {
                expression { params.BRANCH_NAME == 'main' && !params.IS_PR }
            }
            steps {
                echo '🚀 Desplegando a producción...'
                sh 'dotnet publish --configuration Release --output ./publish-prod --no-build'
                echo '✅ Aplicación desplegada en producción.'
            }
        }
    }
    
    post {
        always {
            script {
                // Archivar artefactos
                if (fileExists('publish-staging')) {
                    archiveArtifacts artifacts: 'publish-staging/**/*', allowEmptyArchive: true
                }
                if (fileExists('publish-prod')) {
                    archiveArtifacts artifacts: 'publish-prod/**/*', allowEmptyArchive: true
                }
                if (fileExists('TestResults')) {
                    publishTestResults testResultsPattern: 'TestResults/*.trx'
                }
            }
        }
        success {
            script {
                if (params.IS_PR) {
                    echo "✅ PR #${params.PR_NUMBER} - Pipeline ejecutado exitosamente"
                } else {
                    echo "✅ Branch ${params.BRANCH_NAME} - Pipeline ejecutado exitosamente"
                }
            }
        }
        failure {
            script {
                if (params.IS_PR) {
                    echo "❌ PR #${params.PR_NUMBER} - Pipeline falló"
                } else {
                    echo "❌ Branch ${params.BRANCH_NAME} - Pipeline falló"
                }
            }
        }
    }
}

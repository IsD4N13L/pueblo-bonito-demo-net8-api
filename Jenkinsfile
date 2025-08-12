pipeline {
    agent any

    tools {
        // Asumiendo que el SDK de .NET 8 fue instalado en el sistema
        // y que Jenkins tiene acceso a él.
        // Si necesitas una versión específica, configúrala en "Manage Jenkins" -> "Global Tool Configuration"
    }

    stages {
        stage('Checkout Code') {
            steps {
                echo 'Verificando la conexión con GitHub...'
                // La credencial con el token de GitHub debe estar configurada en el job.
                git url: 'https://github.com/IsD4N13L/pueblo-bonito-demo-net8-api.git', 
                    credentialsId: 'github-token', // Reemplaza 'github-token' con el ID de tu credencial si es diferente
                    branch: 'main'
                echo 'Clonación del repositorio exitosa.'
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
                echo 'Construcción del proyecto exitosa. El sistema está listo.'
            }
        }
    }

    post {
        success {
            echo 'Todos los pasos de validación se completaron correctamente. El sistema está listo para usar.'
        }
        failure {
            echo 'Fallo en la validación. Revisa los logs para identificar el problema.'
        }
    }
}
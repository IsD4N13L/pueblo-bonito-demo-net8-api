pipeline {
    agent any

    stages {
        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore PruebloBonitoApi/'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build PruebloBonitoApi/ --configuration Release'
            }
        }
        stage('Test') {
            steps {
                // Asume que las pruebas están en un proyecto separado dentro de la misma carpeta
                sh 'dotnet test PruebloBonitoApi/ --configuration Release --no-build'
            }
        }
        stage('Publish') {
            steps {
                sh 'dotnet publish PruebloBonitoApi/ --configuration Release --no-build -o ./publish'
            }
        }
    }
}
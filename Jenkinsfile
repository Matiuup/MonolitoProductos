pipeline {
    agent { label 'AgenteWindows' }

    stages {
        stage('Restaurar paquetes NuGet') {
            steps {
                bat 'nuget restore Monolito4B.sln'
            }
        }
        stage('Compilar solucion') {
            steps {
                bat '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" Monolito4B.sln /p:Configuration=Release'
            }
        }
        stage('Publicar aplicacion') {
            steps {
                bat '"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" SistemaProductos/SistemaProductos.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile'
            }
        }
        stage('Desplegar en IIS') {
            steps {
                bat 'xcopy /Y /E "SistemaProductos\\bin\\Release\\Publish\\*" "C:\\inetpub\\wwwroot\\MonolitoApp\\"'
            }
        }
    }

    post {
        success {
            echo 'Pipeline completado exitosamente'
        }
        failure {
            echo 'El pipeline fallo. Revisa los logs.'
        }
    }
}

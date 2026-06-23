pipeline {
    agent { label 'AgenteWindows' }

    environment {
        TOOLS_DIR = "${WORKSPACE}\\.tools"
    }

    stages {
        stage('Preparar herramientas') {
            steps {
                bat '''
                    if not exist "%TOOLS_DIR%" mkdir "%TOOLS_DIR%"
                    if not exist "%TOOLS_DIR%\\nuget.exe" (
                        echo Descargando NuGet...
                        curl.exe -sLo "%TOOLS_DIR%\\nuget.exe" https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
                    )
                '''
            }
        }

        stage('Restaurar paquetes NuGet') {
            steps {
                bat '"%TOOLS_DIR%\\nuget.exe" restore SistemaProductos.sln'
            }
        }

        stage('Compilar solucion') {
            steps {
                bat '"C:\\Program Files\\Microsoft Visual Studio\\18\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe" SistemaProductos.sln /p:Configuration=Release'
            }
        }

        stage('Publicar aplicacion') {
            steps {
                bat '''
                    set MSBUILD="C:\\Program Files\\Microsoft Visual Studio\\18\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe"
                    %MSBUILD% SistemaProductos/SistemaProductos.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile
                '''
            }
        }

        stage('Desplegar en IIS') {
            steps {
                bat '''
                    if not exist "C:\\inetpub\\wwwroot\\MonolitoApp" mkdir "C:\\inetpub\\wwwroot\\MonolitoApp"
                    xcopy /Y /E "SistemaProductos\\bin\\Release\\Publish\\*" "C:\\inetpub\\wwwroot\\MonolitoApp\\"
                '''
            }
        }
    }

    post {
        success {
            echo 'Pipeline completado exitosamente. La aplicacion esta en C:\\inetpub\\wwwroot\\MonolitoApp'
        }
        failure {
            echo 'El pipeline fallo. Revisa los logs.'
        }
    }
}

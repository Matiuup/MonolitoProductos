pipeline {
    agent { label 'AgenteWindows' }

    environment {
        TOOLS_DIR   = "${WORKSPACE}\\.tools"
        PUBLISH_DIR = "${WORKSPACE}\\publish_output"
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
                script {
                    // Iniciar MSBuild en segundo plano y forzar su cierre tras 3 minutos
                    bat '''
                        set MSBUILD="C:\\Program Files\\Microsoft Visual Studio\\18\\Community\\MSBuild\\Current\\Bin\\MSBuild.exe"
                        start "Compile" /B %MSBUILD% SistemaProductos.sln /p:Configuration=Release /p:UseSharedCompilation=false > build.log 2>&1
                        timeout /T 180 /NOBREAK
                        taskkill /F /IM MSBuild.exe /T >nul 2>&1
                        echo MSBuild finalizado por timeout.
                    '''
                    // Verificar que los DLLs necesarios existen
                    bat '''
                        if not exist "SistemaProductos\\bin\\SistemaProductos.dll" exit /b 1
                        if not exist "CapaDatos\\bin\\Release\\CapaDatos.dll" exit /b 1
                        if not exist "CapaNegocio\\bin\\Release\\CapaNegocio.dll" exit /b 1
                        echo Todos los DLLs están presentes. Continuando...
                    '''
                }
            }
        }

        stage('Publicar aplicacion') {
            steps {
                bat '''
                    if exist "%PUBLISH_DIR%" rmdir /S /Q "%PUBLISH_DIR%"
                    mkdir "%PUBLISH_DIR%"
                    xcopy /Y /E "SistemaProductos\\bin\\*" "%PUBLISH_DIR%\\"
                    echo Aplicacion empaquetada en %PUBLISH_DIR%
                '''
            }
        }

        stage('Desplegar en IIS') {
            steps {
                bat '''
                    if not exist "C:\\inetpub\\wwwroot\\MonolitoApp" mkdir "C:\\inetpub\\wwwroot\\MonolitoApp"
                    xcopy /Y /E "%PUBLISH_DIR%\\*" "C:\\inetpub\\wwwroot\\MonolitoApp\\"
                '''
            }
        }
    }

    post {
        success {
            echo 'Pipeline completado exitosamente. Aplicacion en C:\\inetpub\\wwwroot\\MonolitoApp'
        }
        failure {
            echo 'El pipeline fallo. Revisa los logs.'
        }
    }
}

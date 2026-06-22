<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaProductos.Login" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="sm" runat="server" EnablePageMethods="true" />

    <style>
        :root {
            --primary: #2563eb; --primary-dark: #1d4ed8; --primary-light: #3b82f6;
            --secondary: #0f172a; --accent: #06b6d4; --bg-card: rgba(255, 255, 255, 0.98);
            --text-primary: #1e293b; --text-secondary: #64748b; --border-color: #e2e8f0;
        }

        .login-container { position: relative; z-index: 1; width: 100%; max-width: 400px; margin: 2rem auto; }

        .login-card {
            background: var(--bg-card); border-radius: 20px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            padding: 32px 28px; animation: cardEntry 0.5s ease-out;
        }

        @keyframes cardEntry {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .logo-section { text-align: center; margin-bottom: 24px; }
        .logo-icon {
            width: 60px; height: 60px; background: linear-gradient(135deg, var(--primary) 0%, var(--accent) 100%);
            border-radius: 16px; display: flex; align-items: center; justify-content: center;
            margin: 0 auto 16px; box-shadow: 0 8px 20px -5px rgba(37, 99, 235, 0.4);
        }
        .logo-icon i { font-size: 26px; color: white; }
        .login-title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin-bottom: 4px; }
        .login-subtitle { font-size: 14px; color: var(--text-secondary); }

        .form-group { margin-bottom: 16px; }
        .form-label { display: block; font-size: 13px; font-weight: 600; color: var(--text-primary); margin-bottom: 6px; }
        .input-wrapper { position: relative; }
        .input-wrapper i.input-icon {
            position: absolute; left: 14px; top: 50%; transform: translateY(-50%);
            color: var(--text-secondary); font-size: 14px; z-index: 2;
        }
        .form-control {
            width: 100%; padding: 12px 14px 12px 42px; font-size: 14px; font-family: inherit;
            border: 2px solid var(--border-color); border-radius: 10px; background: #f8fafc;
            color: var(--text-primary); transition: all 0.2s ease;
        }
        .form-control:focus {
            outline: none; border-color: var(--primary); background: white;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
        }
        .password-toggle {
            position: absolute; right: 14px; top: 50%; transform: translateY(-50%);
            cursor: pointer; color: var(--text-secondary); background: none; border: none; padding: 4px; z-index: 2;
        }
        .remember-row { display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; flex-wrap: wrap; gap: 8px; }
        .checkbox-wrapper { display: flex; align-items: center; gap: 8px; }
        .checkbox-wrapper input[type="checkbox"] { width: 16px; height: 16px; accent-color: var(--primary); }
        .checkbox-wrapper label { font-size: 13px; color: var(--text-secondary); }
        .link-forgot { font-size: 13px; color: var(--primary); text-decoration: none; font-weight: 500; }
        .link-forgot:hover { text-decoration: underline; }

        .btn-primary-custom {
            width: 100%; padding: 13px 20px; font-size: 15px; font-weight: 600; font-family: inherit;
            color: white; background: linear-gradient(135deg, var(--primary) 0%, var(--primary-dark) 100%);
            border: none; border-radius: 10px; cursor: pointer; transition: all 0.2s ease;
            box-shadow: 0 4px 12px -2px rgba(37, 99, 235, 0.4);
        }
        .btn-primary-custom:hover { transform: translateY(-1px); box-shadow: 0 6px 16px -2px rgba(37, 99, 235, 0.5); }
        .btn-secondary-custom {
            width: 100%; padding: 12px 20px; font-size: 14px; font-weight: 500; font-family: inherit;
            color: var(--text-secondary); background: #f1f5f9; border: 2px solid var(--border-color);
            border-radius: 10px; cursor: pointer; transition: all 0.2s ease;
        }
        .btn-secondary-custom:hover { background: #e2e8f0; }

        .register-link { text-align: center; font-size: 13px; color: var(--text-secondary); margin-top: 15px; }
        .register-link a { color: var(--primary); font-weight: 600; text-decoration: none; }
        .register-link a:hover { text-decoration: underline; }

        /* OTP Panel */
        .otp-section { text-align: center; }
        .otp-icon {
            width: 64px; height: 64px; background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border-radius: 50%; display: flex; align-items: center; justify-content: center;
            margin: 0 auto 20px;
        }
        .otp-icon i { font-size: 28px; color: white; }
        .otp-title { font-size: 20px; font-weight: 700; color: var(--text-primary); margin-bottom: 6px; }
        .otp-subtitle { font-size: 13px; color: var(--text-secondary); margin-bottom: 24px; line-height: 1.5; }
        .otp-input { letter-spacing: 0.4em; font-size: 20px; text-align: center; font-weight: 600; padding: 14px !important; }
        .alert { padding: 12px 14px; border-radius: 10px; font-size: 13px; font-weight: 500; margin-bottom: 16px; display: flex; align-items: center; gap: 10px; }
        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }

        @media (max-width: 420px) {
            .login-card { padding: 24px 20px; }
            .login-title { font-size: 22px; }
        }
    </style>

    <div class="login-container">
        <div class="login-card">
            
            <!-- Panel de Credenciales -->
            <asp:Panel ID="pnlCredenciales" runat="server">
                <div class="logo-section">
                    <div class="logo-icon"><i class="fas fa-shield-halved"></i></div>
                    <h1 class="login-title">Bienvenido</h1>
                    <p class="login-subtitle">Ingresa tus credenciales para continuar</p>
                </div>

                <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert"></asp:Label>

                <div class="form-group">
                    <label class="form-label">Usuario o Correo</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder="ejemplo@correo.com" />
                        <i class="fas fa-user input-icon"></i>
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label">Contraseña</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtClave" runat="server" TextMode="Password" CssClass="form-control" ClientIDMode="Static" placeholder="********" style="padding-right: 42px;" />
                        <i class="fas fa-lock input-icon"></i>
                        <button type="button" class="password-toggle" onclick="togglePassword()"><i id="eyeIcon" class="far fa-eye"></i></button>
                    </div>
                </div>

                <div class="remember-row">
                    <div class="checkbox-wrapper">
                        <asp:CheckBox ID="chkRecordar" runat="server" ClientIDMode="Static" />
                        <label for="chkRecordar">Recordarme</label>
                    </div>
                    <asp:HyperLink ID="lnkRecuperar" runat="server" NavigateUrl="~/RecuperarPassword.aspx" CssClass="link-forgot">Olvidaste tu contraseña?</asp:HyperLink>
                </div>

                <asp:Button ID="btnValidar" runat="server" Text="Iniciar Sesion" CssClass="btn-primary-custom" OnClick="btnValidar_Click" />

                <p class="register-link">
                    No tienes cuenta? <asp:HyperLink ID="lnkRegistro" runat="server" NavigateUrl="~/Registro.aspx">Crear una</asp:HyperLink>
                </p>
            </asp:Panel>

            <!-- Panel OTP -->
            <asp:Panel ID="pnlOTP" runat="server" Visible="false">
                <div class="otp-section">
                    <div class="otp-icon"><i class="fas fa-envelope-open-text"></i></div>
                    <h2 class="otp-title">Verificacion OTP</h2>
                    <p class="otp-subtitle">Hemos enviado un codigo de 8 digitos a tu correo electronico.</p>

                    <asp:Label ID="lblMensajeOTP" runat="server" Visible="false" CssClass="alert"></asp:Label>

                    <div class="form-group">
                        <div class="input-wrapper">
                            <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control otp-input" MaxLength="8" ClientIDMode="Static" placeholder="--------" />
                        </div>
                    </div>

                    <button type="button" id="btnVerificarOTP" class="btn-primary-custom" onclick="verificarOTPAjax()" style="margin-bottom: 10px;"><i class="fas fa-check-circle"></i> Verificar Codigo</button>
                    <asp:Button ID="btnCancelarOTP" runat="server" Text="Cancelar y volver" CssClass="btn-secondary-custom" OnClick="btnCancelarOTP_Click" />
                </div>
            </asp:Panel>

        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script>
        // Función global para mostrar mensajes con SweetAlert2 (toast)
        function showMessage(title, icon) {
            Swal.fire({ title: title, icon: icon, toast: true, position: 'top-end', showConfirmButton: false, timer: 3000 });
        }

        // Función para verificar el OTP vía AJAX
        function verificarOTPAjax() {
            var codigo = document.getElementById("txtOTP").value.trim();
            if (!codigo || codigo.length !== 8) {
                Swal.fire('Error', 'Ingrese el código OTP de 8 caracteres', 'warning');
                return;
            }

            Swal.fire({
                title: 'Verificando OTP...',
                html: '<div class="progress" style="height: 20px;"><div id="progress-bar" class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" style="width: 0%"></div></div>',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => {
                    const progressBar = document.getElementById('progress-bar');
                    let width = 0;
                    const interval = setInterval(() => {
                        width += 5;
                        progressBar.style.width = width + '%';
                        if (width >= 100) clearInterval(interval);
                    }, 150);
                }
            });

            fetch('Login.aspx/VerificarOTP', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ codigo: codigo })
            })
                .then(response => response.json())
                .then(data => {
                    Swal.close();
                    if (data.d && data.d.success) {
                        Swal.fire('¡Éxito!', data.d.message, 'success').then(() => {
                            window.location.href = data.d.redirect;
                        });
                    } else {
                        Swal.fire('Error', data.d.message, 'error');
                    }
                })
                .catch(error => {
                    Swal.close();
                    Swal.fire('Error', 'Error de conexión', 'error');
                });
        }

        // Mostrar/ocultar contraseña
        function togglePassword() {
            const input = document.getElementById('txtClave');
            const icon = document.getElementById('eyeIcon');
            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            }
        }
    </script>
</asp:Content>
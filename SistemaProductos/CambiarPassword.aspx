<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CambiarPassword.aspx.cs" Inherits="SistemaProductos.CambiarPassword" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        :root {
            --primary: #2563eb;
            --primary-dark: #1d4ed8;
            --accent: #06b6d4;
            --bg-card: rgba(255, 255, 255, 0.98);
            --text-primary: #1e293b;
            --text-secondary: #64748b;
            --border-color: #e2e8f0;
        }

        .form-container {
            width: 100%;
            max-width: 420px;
            margin: 2rem auto;
        }

        .form-card {
            background: var(--bg-card);
            border-radius: 20px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            padding: 32px 28px;
            animation: cardEntry 0.5s ease-out;
        }

        @keyframes cardEntry {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }

        .header-section { text-align: center; margin-bottom: 24px; }

        .icon-circle {
            width: 60px;
            height: 60px;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 16px;
            box-shadow: 0 8px 20px -5px rgba(16, 185, 129, 0.4);
        }

        .icon-circle i { font-size: 26px; color: white; }
        .form-title { font-size: 24px; font-weight: 700; color: var(--text-primary); margin-bottom: 4px; }
        .form-subtitle { font-size: 14px; color: var(--text-secondary); }

        .form-group { margin-bottom: 16px; }
        .form-label { display: block; font-size: 13px; font-weight: 600; color: var(--text-primary); margin-bottom: 6px; }

        .input-wrapper { position: relative; }
        .input-wrapper i.input-icon {
            position: absolute;
            left: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--text-secondary);
            font-size: 14px;
            z-index: 2;
        }

        .form-control {
            width: 100%;
            padding: 12px 14px 12px 42px;
            font-size: 14px;
            font-family: inherit;
            border: 2px solid var(--border-color);
            border-radius: 10px;
            background: #f8fafc;
            color: var(--text-primary);
            transition: all 0.2s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary);
            background: white;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
        }

        .password-toggle {
            position: absolute;
            right: 14px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
            color: var(--text-secondary);
            background: none;
            border: none;
            padding: 4px;
            z-index: 2;
        }

        .btn-primary-custom {
            width: 100%;
            padding: 13px 20px;
            font-size: 15px;
            font-weight: 600;
            font-family: inherit;
            color: white;
            background: linear-gradient(135deg, var(--primary) 0%, var(--primary-dark) 100%);
            border: none;
            border-radius: 10px;
            cursor: pointer;
            transition: all 0.2s ease;
            box-shadow: 0 4px 12px -2px rgba(37, 99, 235, 0.4);
        }

        .btn-primary-custom:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 16px -2px rgba(37, 99, 235, 0.5);
        }

        .btn-success-custom {
            width: 100%;
            padding: 13px 20px;
            font-size: 15px;
            font-weight: 600;
            font-family: inherit;
            color: white;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border: none;
            border-radius: 10px;
            cursor: pointer;
            transition: all 0.2s ease;
            box-shadow: 0 4px 12px -2px rgba(16, 185, 129, 0.4);
        }

        .btn-success-custom:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 16px -2px rgba(16, 185, 129, 0.5);
        }

        .alert {
            padding: 12px 14px;
            border-radius: 10px;
            font-size: 13px;
            font-weight: 500;
            margin-bottom: 16px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .alert-danger { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        .alert-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }

        .otp-input {
            letter-spacing: 0.4em;
            font-size: 20px;
            text-align: center;
            font-weight: 600;
            padding: 14px !important;
        }

        .link-back {
            text-align: center;
            margin-top: 20px;
            font-size: 13px;
        }

        .link-back a {
            color: var(--primary);
            text-decoration: none;
            font-weight: 500;
        }

        .link-back a:hover { text-decoration: underline; }
    </style>

    <div class="form-container">
        <div class="form-card">
            <div class="header-section">
                <div class="icon-circle">
                    <i class="fas fa-shield-check"></i>
                </div>
                <h1 class="form-title">Cambiar Contraseña</h1>
                <p class="form-subtitle" id="subtitleText">Ingresa el código enviado a tu correo</p>
            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert"></asp:Label>

            <!-- Paso 1: Ingresar el token de 6 dígitos -->
            <asp:Panel ID="pnlToken" runat="server">
                <div class="form-group">
                    <label class="form-label">Código de verificación</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtToken" runat="server" CssClass="form-control otp-input" MaxLength="6" placeholder="000000" />
                        <i class="fas fa-hashtag input-icon"></i>
                    </div>
                </div>
                <asp:Button ID="btnValidarToken" runat="server" Text="Validar código" CssClass="btn-primary-custom" OnClick="btnValidarToken_Click" />
            </asp:Panel>

            <!-- Paso 2: Campos para la nueva contraseña (oculto hasta validar token) -->
            <asp:Panel ID="pnlNueva" runat="server" Visible="false">
                <div class="form-group">
                    <label class="form-label">Nueva contraseña</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtNueva" runat="server" CssClass="form-control" TextMode="Password" ClientIDMode="Static" style="padding-right: 42px;" placeholder="Mínimo 8 caracteres" />
                        <i class="fas fa-lock input-icon"></i>
                        <button type="button" class="password-toggle" onclick="togglePassword('txtNueva', 'eyeNueva')">
                            <i id="eyeNueva" class="far fa-eye"></i>
                        </button>
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Confirmar contraseña</label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtConfirmar" runat="server" CssClass="form-control" TextMode="Password" ClientIDMode="Static" style="padding-right: 42px;" placeholder="Repite la contraseña" />
                        <i class="fas fa-lock input-icon"></i>
                        <button type="button" class="password-toggle" onclick="togglePassword('txtConfirmar', 'eyeConfirmar')">
                            <i id="eyeConfirmar" class="far fa-eye"></i>
                        </button>
                    </div>
                </div>
                <asp:Button ID="btnCambiar" runat="server" Text="Actualizar contraseña" CssClass="btn-success-custom" OnClick="btnCambiar_Click" />
            </asp:Panel>

            <div class="link-back">
                <asp:HyperLink ID="lnkVolver" runat="server">Volver</asp:HyperLink>
            </div>
        </div>
    </div>

    <script>
        // Función para mostrar/ocultar la contraseña en los campos de texto
        function togglePassword(fieldId, iconId) {
            const input = document.getElementById(fieldId);
            const icon = document.getElementById(iconId);
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
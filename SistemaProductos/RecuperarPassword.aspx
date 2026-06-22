<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperarPassword.aspx.cs" Inherits="SistemaProductos.RecuperarPassword" MasterPageFile="~/Site.Master" %>

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
            background: linear-gradient(135deg, var(--primary) 0%, var(--accent) 100%);
            border-radius: 16px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 16px;
            box-shadow: 0 8px 20px -5px rgba(37, 99, 235, 0.4);
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
                    <i class="fas fa-key"></i>
                </div>
                <h1 class="form-title">Recuperar Contraseña</h1>
                <p class="form-subtitle">Ingresa tu correo electrónico registrado</p>
            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert"></asp:Label>

            <div class="form-group">
                <label class="form-label">Correo electrónico</label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" TextMode="Email" placeholder="tu@correo.com" />
                    <i class="fas fa-envelope input-icon"></i>
                </div>
            </div>

            <asp:Button ID="btnEnviar" runat="server" Text="Enviar código de recuperación" CssClass="btn-primary-custom" OnClick="btnEnviar_Click" />

            <div class="link-back">
                <asp:HyperLink ID="lnkVolverLogin" runat="server" NavigateUrl="~/Login.aspx">Volver al inicio de sesión</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
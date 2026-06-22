<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="SistemaProductos.Perfil" MasterPageFile="~/Site.Master" %>

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

        .profile-container {
            width: 100%;
            max-width: 580px;
            margin: 2rem auto;
        }

        .profile-card {
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

        .avatar-section {
            display: flex;
            flex-direction: column;
            align-items: center;
            margin-bottom: 24px;
        }

        .avatar {
            width: 120px;
            height: 120px;
            object-fit: cover;
            border-radius: 50%;
            border: 4px solid white;
            box-shadow: 0 8px 20px rgba(0,0,0,0.15);
        }

        .avatar-placeholder {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            background: linear-gradient(135deg, var(--primary) 0%, var(--accent) 100%);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
            color: white;
        }

        .user-name {
            font-size: 20px;
            font-weight: 700;
            color: var(--text-primary);
            margin-top: 12px;
        }

        .user-email {
            font-size: 14px;
            color: var(--text-secondary);
            margin-top: 4px;
        }

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
            margin-top: 8px;
        }

        .btn-primary-custom:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 16px -2px rgba(37, 99, 235, 0.5);
        }

        .btn-outline-custom {
            width: 100%;
            padding: 12px 20px;
            font-size: 14px;
            font-weight: 500;
            font-family: inherit;
            color: var(--primary);
            background: white;
            border: 2px solid var(--border-color);
            border-radius: 10px;
            cursor: pointer;
            transition: all 0.2s ease;
            text-align: center;
            display: block;
        }

        .btn-outline-custom:hover {
            background: #f8fafc;
            border-color: var(--primary);
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
        .alert-warning { background: #fffbeb; color: #d97706; border: 1px solid #fde68a; }

        .file-upload-wrapper {
            display: flex;
            gap: 8px;
            margin-bottom: 10px;
        }

        .file-upload-wrapper .form-control {
            padding-left: 14px;
        }

        .preview-image {
            width: 100%;
            max-height: 200px;
            object-fit: contain;
            border-radius: 10px;
            border: 2px solid var(--border-color);
            margin-top: 10px;
        }

        @media (max-width: 420px) {
            .profile-card { padding: 24px 20px; }
        }
    </style>

    <div class="profile-container">
        <div class="profile-card">
            <!-- Avatar y datos básicos -->
            <div class="avatar-section">
                <asp:Image ID="imgPerfil" runat="server" CssClass="avatar" Visible="false" />
                <div id="avatarPlaceholder" class="avatar-placeholder" runat="server">
                    <i class="fas fa-user"></i>
                </div>
                <h4 class="user-name"><asp:Label ID="lblNombreCompleto" runat="server" /></h4>
                <p class="user-email"><asp:Label ID="lblCorreo" runat="server" /></p>
            </div>

            <asp:Label ID="lblMensaje" runat="server" Visible="false" CssClass="alert"></asp:Label>

            <!-- Campos de texto -->
            <div class="form-group">
                <label class="form-label">Nombres</label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" placeholder="Tus nombres" />
                    <i class="fas fa-user input-icon"></i>
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Apellidos</label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" placeholder="Tus apellidos" />
                    <i class="fas fa-user input-icon"></i>
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Celular</label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtCelular" runat="server" CssClass="form-control" placeholder="0991234567" />
                    <i class="fas fa-phone input-icon"></i>
                </div>
            </div>

            <!-- Subida de imagen -->
            <div class="form-group">
                <label class="form-label">Cambiar foto de perfil (máx. 2MB, JPG/PNG/GIF)</label>
                <div class="file-upload-wrapper">
                    <asp:FileUpload ID="fuImagen" runat="server" CssClass="form-control" accept=".jpg,.jpeg,.png,.gif" />
                    <asp:Button ID="btnPrevisualizar" runat="server" Text="Previsualizar" CssClass="btn-outline-custom" OnClick="btnPrevisualizar_Click" style="width: auto;" />
                </div>
                <asp:Image ID="imgPreview" runat="server" CssClass="preview-image" Visible="false" />
                <asp:Button ID="btnSubirImagen" runat="server" Text="Guardar foto" CssClass="btn-primary-custom" OnClick="btnSubirImagen_Click" Visible="false" />
            </div>

            <asp:Button ID="btnGuardar" runat="server" Text="Guardar cambios" CssClass="btn-primary-custom" OnClick="btnGuardar_Click" />
            <asp:HyperLink ID="lnkCambiarPassword" runat="server" CssClass="btn-outline-custom" style="margin-top: 10px;">Cambiar contraseña</asp:HyperLink>
        </div>
    </div>
</asp:Content>
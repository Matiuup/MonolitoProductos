<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="SistemaProductos.Registro" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .reg-wrap {
            min-height: 90vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 2rem 1rem;
        }

        .reg-card {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 8px 40px rgba(0,0,0,0.13);
            padding: 2rem 2rem 1.5rem;
            width: 100%;
            max-width: 520px;
        }

        .reg-header {
            text-align: center;
            margin-bottom: 1.5rem;
        }

        .reg-header .icon-wrap {
            width: 52px;
            height: 52px;
            background: linear-gradient(135deg, #10b981, #059669);
            border-radius: 14px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 0.75rem;
            box-shadow: 0 4px 14px rgba(16,185,129,0.35);
        }

        .reg-header .icon-wrap i { color: #fff; font-size: 22px; }
        .reg-header h2 { font-size: 1.4rem; font-weight: 700; color: #1e293b; margin: 0; }
        .reg-header p  { font-size: 0.8rem; color: #64748b; margin: 2px 0 0; }

        .sec-label {
            font-size: 0.7rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.6px;
            color: #2563eb;
            border-bottom: 1.5px solid #e2e8f0;
            padding-bottom: 4px;
            margin: 1.1rem 0 0.6rem;
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .form-grid-2 {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 0.6rem;
        }

        .field { margin-bottom: 0.6rem; }

        .field label {
            font-size: 0.72rem;
            font-weight: 600;
            color: #334155;
            display: block;
            margin-bottom: 3px;
        }

        .field label .req { color: #ef4444; }
        .field label .opt { color: #94a3b8; font-weight: 400; }

        .input-wrap { position: relative; }

        .input-wrap .ico {
            position: absolute;
            left: 9px;
            top: 50%;
            transform: translateY(-50%);
            color: #94a3b8;
            font-size: 12px;
            pointer-events: none;
        }

        .input-wrap input,
        .input-wrap select {
            width: 100%;
            padding: 8px 8px 8px 30px;
            font-size: 0.8rem;
            border: 1.5px solid #e2e8f0;
            border-radius: 8px;
            background: #f8fafc;
            color: #1e293b;
            transition: border-color 0.2s, box-shadow 0.2s;
            font-family: inherit;
        }

        .input-wrap input:focus,
        .input-wrap select:focus {
            outline: none;
            border-color: #2563eb;
            background: #fff;
            box-shadow: 0 0 0 3px rgba(37,99,235,0.08);
        }

        .input-wrap input.sugerido {
            background: #ecfdf5;
            border-color: #10b981;
        }

        .input-wrap .eye-btn {
            position: absolute;
            right: 8px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            color: #94a3b8;
            cursor: pointer;
            font-size: 12px;
            padding: 2px;
        }

        .input-wrap .eye-btn:hover { color: #2563eb; }

        .hint {
            font-size: 0.68rem;
            color: #94a3b8;
            margin-top: 2px;
            display: none;
            align-items: center;
            gap: 3px;
        }

        .hint.ok { color: #10b981; }

        /* Barra de fuerza */
        .strength-wrap {
            margin-top: 4px;
            height: 3px;
            background: #e2e8f0;
            border-radius: 2px;
            overflow: hidden;
        }

        .strength-bar {
            height: 100%;
            width: 0;
            transition: width 0.3s, background 0.3s;
            border-radius: 2px;
        }

        /* Foto de perfil */
        .photo-section {
            display: flex;
            align-items: center;
            gap: 1rem;
            background: #f8fafc;
            border: 1.5px dashed #e2e8f0;
            border-radius: 10px;
            padding: 0.75rem;
            margin-top: 0.6rem;
        }

        .photo-circle {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            background: #e2e8f0;
            display: flex;
            align-items: center;
            justify-content: center;
            overflow: hidden;
            flex-shrink: 0;
            border: 2px solid #fff;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }

        .photo-circle i { font-size: 22px; color: #94a3b8; }
        .photo-circle img { width: 100%; height: 100%; object-fit: cover; }

        .photo-controls p { font-size: 0.72rem; color: #64748b; margin: 0 0 6px; }

        .btn-upload-label {
            display: inline-block;
            padding: 5px 10px;
            font-size: 0.72rem;
            font-weight: 600;
            color: #2563eb;
            background: #fff;
            border: 1.5px solid #2563eb;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s;
            margin-right: 4px;
        }

        .btn-upload-label:hover { background: #2563eb; color: #fff; }

        .btn-prev-img {
            display: inline-block;
            padding: 5px 10px;
            font-size: 0.72rem;
            font-weight: 600;
            color: #fff;
            background: #06b6d4;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            transition: background 0.2s;
        }

        .btn-prev-img:hover { background: #0891b2; }

        /* Mensaje */
        .msg-box {
            padding: 8px 12px;
            border-radius: 8px;
            font-size: 0.78rem;
            font-weight: 500;
            margin-bottom: 0.8rem;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .msg-danger  { background: #fef2f2; color: #dc2626; border: 1px solid #fecaca; }
        .msg-success { background: #f0fdf4; color: #16a34a; border: 1px solid #bbf7d0; }
        .msg-warning { background: #fffbeb; color: #d97706; border: 1px solid #fde68a; }

        /* Botón principal */
        .btn-registrar {
            width: 100%;
            padding: 11px;
            font-size: 0.9rem;
            font-weight: 700;
            color: #fff;
            background: linear-gradient(135deg, #10b981, #059669);
            border: none;
            border-radius: 10px;
            cursor: pointer;
            transition: all 0.2s;
            box-shadow: 0 4px 14px rgba(16,185,129,0.3);
            margin-top: 1rem;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            font-family: inherit;
        }

        .btn-registrar:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 20px rgba(16,185,129,0.4);
        }

        .divider {
            display: flex;
            align-items: center;
            gap: 10px;
            margin: 1rem 0 0.6rem;
        }

        .divider hr { flex: 1; border: none; border-top: 1px solid #e2e8f0; }
        .divider span { font-size: 0.72rem; color: #94a3b8; }

        .login-link {
            text-align: center;
            font-size: 0.78rem;
            color: #64748b;
        }

        .login-link a { color: #2563eb; font-weight: 600; text-decoration: none; }
        .login-link a:hover { text-decoration: underline; }

        .file-hidden { display: none; }

        @media (max-width: 540px) {
            .form-grid-2 { grid-template-columns: 1fr; }
            .reg-card { padding: 1.5rem 1rem; }
        }
    </style>

    <div class="reg-wrap">
        <div class="reg-card">

            <div class="reg-header">
                <div class="icon-wrap"><i class="fas fa-user-plus"></i></div>
                <h2>Crear Cuenta</h2>
                <p>Completa tus datos para registrarte</p>
            </div>

            <%-- Mensaje de alerta --%>
            <asp:Label ID="lblMensaje" runat="server" Visible="false" />

            <%-- Campos ocultos para preservar contraseña en postback --%>
            <input type="hidden" id="hdnClave"    name="hdnClave" />
            <input type="hidden" id="hdnConfirmar" name="hdnConfirmar" />

            <!-- Datos Personales -->
            <div class="sec-label"><i class="fas fa-user"></i> Datos Personales</div>

            <div class="form-grid-2">
                <div class="field">
                    <label>Nombres <span class="req">*</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-user ico"></i>
                        <asp:TextBox ID="txtNombres" runat="server" ClientIDMode="Static"
                            placeholder="Ej: Juan Carlos" onblur="generarSugerencias()" />
                    </div>
                </div>
                <div class="field">
                    <label>Apellidos <span class="req">*</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-user ico"></i>
                        <asp:TextBox ID="txtApellidos" runat="server" ClientIDMode="Static"
                            placeholder="Ej: Pérez García" onblur="generarSugerencias()" />
                    </div>
                </div>
            </div>

            <div class="form-grid-2">
                <div class="field">
                    <label>Cédula <span class="opt">(opcional)</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-id-card ico"></i>
                        <asp:TextBox ID="txtCedula" runat="server" ClientIDMode="Static"
                            placeholder="0000000000" MaxLength="10" />
                    </div>
                </div>
                <div class="field">
                    <label>Fecha de Nacimiento <span class="opt">(opcional)</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-calendar ico"></i>
                        <asp:TextBox ID="txtFechaCumple" runat="server" ClientIDMode="Static"
                            TextMode="Date" />
                    </div>
                </div>
            </div>

            <!-- Contacto -->
            <div class="sec-label"><i class="fas fa-address-book"></i> Contacto</div>

            <div class="form-grid-2">
                <div class="field">
                    <label>Correo <span class="opt">(sugerido)</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-envelope ico"></i>
                        <asp:TextBox ID="txtCorreo" runat="server" ClientIDMode="Static"
                            TextMode="Email" placeholder="Se genera automáticamente" />
                    </div>
                    <div id="hintCorreo" class="hint ok"><i class="fas fa-check-circle"></i> Correo sugerido</div>
                </div>
                <div class="field">
                    <label>Celular <span class="opt">(opcional)</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-phone ico"></i>
                        <asp:TextBox ID="txtCelular" runat="server" ClientIDMode="Static"
                            placeholder="0999999999" MaxLength="10" />
                    </div>
                </div>
            </div>

            <div class="field">
                <label>Dirección <span class="opt">(opcional)</span></label>
                <div class="input-wrap">
                    <i class="fas fa-map-marker-alt ico"></i>
                    <asp:TextBox ID="txtDireccion" runat="server" ClientIDMode="Static"
                        placeholder="Calle Principal #123" />
                </div>
            </div>

            <!-- Credenciales -->
            <div class="sec-label"><i class="fas fa-key"></i> Credenciales</div>

            <div class="field">
                <label>Nickname <span class="req">*</span> <span class="opt">(sugerido)</span></label>
                <div class="input-wrap">
                    <i class="fas fa-at ico"></i>
                    <asp:TextBox ID="txtNick" runat="server" ClientIDMode="Static"
                        placeholder="Se genera automáticamente" />
                </div>
                <div id="hintNick" class="hint ok"><i class="fas fa-check-circle"></i> Nickname sugerido</div>
            </div>

            <div class="form-grid-2">
                <div class="field">
                    <label>Contraseña <span class="req">*</span> <span class="opt">(sugerida)</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-lock ico"></i>
                        <asp:TextBox ID="txtClave" runat="server" TextMode="Password"
                            ClientIDMode="Static" placeholder="Mín. 8 caracteres"
                            onkeyup="medirFuerza()" style="padding-right:28px;" />
                        <button type="button" class="eye-btn" onclick="verPassword('txtClave','ojo1')">
                            <i id="ojo1" class="far fa-eye"></i>
                        </button>
                    </div>
                    <div class="strength-wrap">
                        <div id="barraFuerza" class="strength-bar"></div>
                    </div>
                    <div id="hintClave" class="hint ok"><i class="fas fa-check-circle"></i> Contraseña sugerida</div>
                </div>
                <div class="field">
                    <label>Confirmar <span class="req">*</span></label>
                    <div class="input-wrap">
                        <i class="fas fa-lock ico"></i>
                        <asp:TextBox ID="txtConfirmarClave" runat="server" TextMode="Password"
                            ClientIDMode="Static" placeholder="Repite la contraseña"
                            style="padding-right:28px;" />
                        <button type="button" class="eye-btn" onclick="verPassword('txtConfirmarClave','ojo2')">
                            <i id="ojo2" class="far fa-eye"></i>
                        </button>
                    </div>
                </div>
            </div>
            <p style="font-size:0.68rem;color:#94a3b8;margin-bottom:0.5rem;">
                <i class="fas fa-info-circle"></i> Mín. 8 caracteres, mayúscula, minúscula, número y símbolo
            </p>

            <!-- Foto de Perfil -->
            <div class="sec-label"><i class="fas fa-camera"></i> Foto de Perfil <span class="opt" style="text-transform:none;font-weight:400;">(opcional)</span></div>

            <div class="photo-section">
                <div class="photo-circle" id="photoCircle">
                    <asp:Image ID="imgPreview" runat="server" Visible="false" />
                    <i class="fas fa-user" id="iconPlaceholder" runat="server"></i>
                </div>
                <div class="photo-controls">
                    <p>Solo PNG o JPG · Máx. 4MB</p>
                    <asp:FileUpload ID="fuFoto" runat="server" CssClass="file-hidden"
                        ClientIDMode="Static" accept=".png,.jpg,.jpeg" />
                    <label for="fuFoto" class="btn-upload-label">
                        <i class="fas fa-upload"></i> Seleccionar
                    </label>
                    <asp:Button ID="btnPrevisualizar" runat="server" Text="Ver foto"
                        CssClass="btn-prev-img" OnClick="btnPrevisualizar_Click"
                        OnClientClick="copiarClaves();" />
                </div>
            </div>

            <!-- Botón registrar -->
            <asp:Button ID="btnRegistrar" runat="server" Text="Crear mi cuenta"
                CssClass="btn-registrar" OnClick="btnRegistrar_Click"
                CausesValidation="false" OnClientClick="copiarClaves();" />

            <div class="divider">
                <hr /><span>o</span><hr />
            </div>

            <p class="login-link">
                ¿Ya tienes cuenta?
                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx">
                    Iniciar sesión
                </asp:HyperLink>
            </p>

        </div>
    </div>

    <script>
        // ── COPIAR CLAVES A OCULTOS (antes de cualquier postback) ──────
        function copiarClaves() {
            var c = document.getElementById('txtClave');
            var cf = document.getElementById('txtConfirmarClave');
            var hc = document.getElementById('hdnClave');
            var hf = document.getElementById('hdnConfirmar');
            if (c && hc) hc.value = c.value;
            if (cf && hf) hf.value = cf.value;
            return true;
        }

        // ── VER/OCULTAR CONTRASEÑA ──────────────────────────────────────
        function verPassword(inputId, iconId) {
            var inp = document.getElementById(inputId);
            var icon = document.getElementById(iconId);
            if (!inp) return;
            if (inp.type === 'password') {
                inp.type = 'text';
                if (icon) { icon.classList.remove('fa-eye'); icon.classList.add('fa-eye-slash'); }
            } else {
                inp.type = 'password';
                if (icon) { icon.classList.remove('fa-eye-slash'); icon.classList.add('fa-eye'); }
            }
        }

        // ── BARRA DE FUERZA ─────────────────────────────────────────────
        function medirFuerza() {
            var pwd = document.getElementById('txtClave').value;
            var barra = document.getElementById('barraFuerza');
            if (!barra) return;
            var pts = 0;
            if (pwd.length >= 8) pts++;
            if (/[A-Z]/.test(pwd)) pts++;
            if (/[a-z]/.test(pwd)) pts++;
            if (/[0-9]/.test(pwd)) pts++;
            if (/[^A-Za-z0-9]/.test(pwd)) pts++;

            var pct = (pts / 5) * 100;
            var color = pts <= 2 ? '#ef4444' : pts <= 3 ? '#f59e0b' : '#10b981';
            barra.style.width = pct + '%';
            barra.style.background = color;
        }

        // ── SUGERENCIAS AUTOMÁTICAS ─────────────────────────────────────
        function generarSugerencias() {
            try {
                var noms = (document.getElementById('txtNombres').value || '').trim();
                var aps = (document.getElementById('txtApellidos').value || '').trim();
                var arrN = noms.split(/\s+/).filter(Boolean);
                var arrA = aps.split(/\s+/).filter(Boolean);

                if (arrN.length < 2 || arrA.length < 2) return;

                var norm = function (s) {
                    return s.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');
                };

                var n1 = norm(arrN[0]);
                var a1 = norm(arrA[0]);
                var a2 = norm(arrA[1]);
                var num = Math.floor(Math.random() * 99) + 1;

                // Nick
                var elNick = document.getElementById('txtNick');
                if (elNick && elNick.value === '') {
                    elNick.value = n1 + a1.charAt(0) + a2.charAt(0) + num;
                    elNick.classList.add('sugerido');
                    mostrarHint('hintNick');
                }

                // Correo
                var elCorreo = document.getElementById('txtCorreo');
                if (elCorreo && elCorreo.value === '') {
                    elCorreo.value = n1 + '.' + a1 + '@gmail.com';
                    elCorreo.classList.add('sugerido');
                    mostrarHint('hintCorreo');
                }

                // Contraseña
                var elClave = document.getElementById('txtClave');
                var elConfirmar = document.getElementById('txtConfirmarClave');
                if (elClave && elClave.value === '') {
                    var pwd = generarPassword(n1, a1);
                    elClave.value = pwd;
                    elClave.classList.add('sugerido');
                    if (elConfirmar) elConfirmar.value = pwd;
                    mostrarHint('hintClave');
                    medirFuerza();
                }
            } catch (ex) {
                console.warn('generarSugerencias:', ex.message);
            }
        }

        function generarPassword(nombre, apellido) {
            var especiales = ['@', '#', '$', '!', '&', '*'];
            var esp = especiales[Math.floor(Math.random() * especiales.length)];
            return nombre.charAt(0).toUpperCase() +
                apellido.toLowerCase() +
                Math.floor(Math.random() * 900 + 100) +
                esp;
        }

        function mostrarHint(id) {
            var el = document.getElementById(id);
            if (el) el.style.display = 'flex';
        }

        // ── VALIDAR FOTO ANTES DE SUBIR ─────────────────────────────────
        window.addEventListener('DOMContentLoaded', function () {
            var fu = document.getElementById('fuFoto');
            if (!fu) return;
            fu.addEventListener('change', function () {
                if (!this.files || !this.files[0]) return;
                var file = this.files[0];
                var ext = file.name.split('.').pop().toLowerCase();
                var mb = file.size / 1024 / 1024;
                if (!['png', 'jpg', 'jpeg'].includes(ext)) {
                    alert('Solo se permiten imágenes PNG o JPG.');
                    this.value = '';
                    return;
                }
                if (mb > 4) {
                    alert('La imagen no puede superar 4MB.');
                    this.value = '';
                }
            });
        });
    </script>

</asp:Content>

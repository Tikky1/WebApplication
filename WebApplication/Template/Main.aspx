<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="WebApplication.Template.Main" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>



    <meta charset="utf-8">
    <meta content="width=device-width, initial-scale=1.0" name="viewport">
    <title>Zeus</title>
    <meta name="description" content="">
    <meta name="keywords" content="">

    <!-- Favicons -->
    <link href="assets/img/favicon.png" rel="icon">
    <link href="assets/img/apple-touch-icon.png" rel="apple-touch-icon">

    <!-- Fonts -->
    <link href="https://fonts.googleapis.com" rel="preconnect">
    <link href="https://fonts.gstatic.com" rel="preconnect" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Roboto:ital,wght@0,100;0,300;0,400;0,500;0,700;0,900;1,100;1,300;1,400;1,500;1,700;1,900&family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&family=Raleway:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap" rel="stylesheet">

    <!-- Vendor CSS Files -->
    <link href="assets/vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">
    <link href="assets/vendor/bootstrap-icons/bootstrap-icons.css" rel="stylesheet">
    <link href="assets/vendor/aos/aos.css" rel="stylesheet">
    <link href="assets/vendor/animate.css/animate.min.css" rel="stylesheet">
    <link href="assets/vendor/glightbox/css/glightbox.min.css" rel="stylesheet">
    <link href="assets/vendor/swiper/swiper-bundle.min.css" rel="stylesheet">

    <!-- Main CSS File -->
    <link href="assets/css/main.css" rel="stylesheet">

    <!-- =======================================================
  * Template Name: Selecao
  * Template URL: https://bootstrapmade.com/selecao-bootstrap-template/
  * Updated: Aug 07 2024 with Bootstrap v5.3.3
  * Author: BootstrapMade.com
  * License: https://bootstrapmade.com/license/
  ======================================================== -->


    <script src="Scripts/jquery-3.6.0.min.js"></script>

</head>

<body class="Main-page">

    <header id="header" class="header d-flex align-items-center fixed-top">
        <div class="container-fluid container-xl position-relative d-flex align-items-center justify-content-between">

            <a href="Main.aspx" class="logo d-flex align-items-center">
                <!-- Uncomment the line below if you also wish to use an image logo -->
                <!-- <img src="assets/img/logo.png" alt=""> -->
                <h1 class="sitename">Zeus</h1>
            </a>

            <nav id="navmenu" class="navmenu">
                <ul>
                    <li><a href="LogoutPage.aspx">Exit</a></li>
                </ul>
                <i class="mobile-nav-toggle d-xl-none bi bi-list"></i>
            </nav>

        </div>
    </header>

    <main class="main">
        <!-- Page Title -->
        <div class="page-title dark-background">
            <div class="container position-relative">
                <h1>Home</h1>
                <nav class="breadcrumbs">
                </nav>
            </div>
        </div>
        <!-- End Page Title -->
    </main>


    <!-- Content -->

    <form id="Form1" runat="server" class="main-panel">
        <!-- Hava Durumu Paneli -->
        <div class="weather-panel" style="margin: 100px auto; width: 300px; text-align: center;">
            <h2>Hava Durumu</h2>
            <div class="search-bar">
                <asp:TextBox ID="txtCity" runat="server" placeholder="Şehir adı girin"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" ErrorMessage="Şehir adı gereklidir." ValidationGroup="WeatherSearch" Display="Dynamic" ForeColor="Red" />
                <asp:Button ID="btnSearch" runat="server" Text="Ara" OnClick="btnSearch_Click" ValidationGroup="WeatherSearch" />
            </div>
            <div class="weather-info">
                <asp:Label ID="lblWeatherInfo" runat="server" Text=""></asp:Label>
            </div>
        </div>

        <!-- Yorum Paneli -->
        <div style="margin: 20px;">
            <!-- Kullanıcı Yorumu Girişi -->
            <h3>Yorum Ekle</h3>
            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="4" Width="100%" Placeholder="Yorumunuzu buraya yazın"></asp:TextBox>
            <asp:Button ID="btnAddComment" runat="server" Text="Yorumu Kaydet" OnClick="btnAddComment_Click" />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>

            <hr />

            <!-- Yorumlar Listesi -->
            <h3>Yorumlar</h3>
            <asp:Repeater ID="rptComments" runat="server" OnItemCommand="rptComments_ItemCommand">
                <ItemTemplate>
                    <div style="margin-bottom: 20px; padding: 10px; border: 1px solid #ccc;">
                        <strong><%# Eval("email") %> (<%# Eval("City") %>):</strong>
                        <p><%# Eval("CommentText") %></p>
                        <span style="font-size: small; color: gray;"><%# Eval("CreatedAt") %></span>

                        <!-- Düzenleme Butonu (Kendi Mesajı veya Admin için Görünür) -->
                        <asp:Button ID="btnEdit" runat="server" Text="Düzenle"
                            CommandName="EditComment" CommandArgument='<%# Eval("id") %>'
                            Visible='<%# Eval("email").ToString() == Session["User"].ToString() || IsAdmin(Session["User"].ToString()) %>' />
                        <!-- DeActive Butonu (Kendi Mesajı veya Admin için Görünür) -->
                        <asp:Button ID="btnDeActive" runat="server" Text="Sil"
                            CommandName="DeActiveComment" CommandArgument='<%# Eval("id") %>'
                            Visible='<%# Eval("email").ToString() == Session["User"].ToString() || IsAdmin(Session["User"].ToString()) %>' />
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlEditComment" runat="server" Visible="false">
                <h3>Yorumu Düzenle</h3>
                <asp:TextBox ID="txtEditComment" runat="server" TextMode="MultiLine" Rows="4" Width="100%"></asp:TextBox>
                <asp:HiddenField ID="hfCommentId" runat="server" />
                <asp:Button ID="btnSaveComment" runat="server" Text="Kaydet" OnClick="btnSaveComment_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="İptal" OnClick="btnCancelEdit_Click" />
            </asp:Panel>




        </div>
    </form>









    <footer id="footer" class="footer dark-background">
        <div class="container">
            <h3 class="sitename">Zeus</h3>
            <p>Fakat birisi kurtaracak gelip bi' gün Atam gibi</p>
            <p>-Hidra</p>
            <div class="social-links d-flex justify-content-center">
                <a href=""><i class="bi bi-twitter-x"></i></a>
                <a href=""><i class="bi bi-facebook"></i></a>
                <a href=""><i class="bi bi-instagram"></i></a>
                <a href=""><i class="bi bi-skype"></i></a>
                <a href=""><i class="bi bi-linkedin"></i></a>
            </div>
            <div class="container">
                <div class="copyright">
                    <span>Copyright</span> <strong class="px-1 sitename">Selecao</strong> <span>All Rights Reserved</span>
                </div>
                <div class="credits">
                    <!-- All the links in the footer should remain intact. -->
                    <!-- You can delete the links only if you've purchased the pro version. -->
                    <!-- Licensing information: https://bootstrapmade.com/license/ -->
                    <!-- Purchase the pro version with working PHP/AJAX contact form: [buy-url] -->
                    Designed by <a href="https://bootstrapmade.com/">BootstrapMade</a> Distributed By <a href="https://themewagon.com">ThemeWagon</a>
                </div>
            </div>
        </div>
    </footer>

    <!-- Scroll Top -->
    <a href="#" id="scroll-top" class="scroll-top d-flex align-items-center justify-content-center"><i class="bi bi-arrow-up-short"></i></a>

    <!-- Preloader -->
    <div id="preloader"></div>

    <!-- Vendor JS Files -->
    <script src="assets/vendor/bootstrap/js/bootstrap.bundle.min.js"></script>
    <script src="assets/vendor/php-email-form/validate.js"></script>
    <script src="assets/vendor/aos/aos.js"></script>
    <script src="assets/vendor/glightbox/js/glightbox.min.js"></script>
    <script src="assets/vendor/imagesloaded/imagesloaded.pkgd.min.js"></script>
    <script src="assets/vendor/isotope-layout/isotope.pkgd.min.js"></script>
    <script src="assets/vendor/swiper/swiper-bundle.min.js"></script>

    <!-- Main JS File -->
    <script src="assets/js/main.js"></script>

</body>
</html>

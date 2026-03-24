<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ForgotPass.aspx.cs" Inherits="ForgotPass" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  

     <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
     <title>TindaSoft-POS</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta name="robots" content="all,follow">

    
         <style type="text/css">
        .messagealert {
            width: 100%;
            position: fixed;
             top:0px;
            z-index: 100000;
            padding: 0;
            font-size: 15px;
        }
    </style>

         <script type="text/javascript">
             function ShowMessage(message, messagetype) {
                 var cssclass;
                 switch (messagetype) {
                     case 'Success':
                         cssclass = 'alert-success'
                         break;
                     case 'Error':
                         cssclass = 'alert-danger'
                         break;
                     case 'Warning':
                         cssclass = 'alert-warning'
                         break;
                     default:
                         cssclass = 'alert-info'
                 }
                 $('#alert_container').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>' + messagetype + '!</strong> <span>' + message + '</span></div>');

                 setTimeout(function () {
                     $("#alert_div").fadeTo(2000, 500).slideUp(500, function () {
                         $("#alert_div").remove();
                     });
                 }, 1000); //5000=5 seconds
             }
    </script>

    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="distribution/vendor/bootstrap/css/bootstrap.min.css">
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="distribution/vendor/font-awesome/css/font-awesome.min.css">
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="distribution/css/fontastic.css">
    <!-- Google fonts - Poppins -->
  
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="distribution/css/style.pink.css" />
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="distribution/css/custom.css">
    <!-- Favicon-->
      <link rel="shortcut icon" href="distribution/img/logopo.ico">

<link rel="stylesheet" href="distribution/fonts/gfont/gfont.css">
</head>
<body>
    <form id="form1" runat="server">
     <div class="page login-page">
      <div class="container d-flex align-items-center">
        <div class="form-holder has-shadow">
          <div class="row">
            <!-- Logo & Information Panel-->
            <div class="col-lg-6">
              <div class="info d-flex align-items-center">
                <div class="content">
                  <div class="logo">
                    <h1>TINDASOFT | POS </h1>
                  </div>
                <p>We serve you for free,   <a href="Register.aspx" class="forgot-pass" style="color: #FFFFFF; text-decoration: underline;">Register Now</a></p> 
                </div>
              </div>
            </div>
            <!-- Form Panel    -->
          <div class="col-lg-6 bg-white">
              <div class="form d-flex align-items-center">
                <div class="content">
                  <div class="form-validate">
                        <label for="register-agree">Recover your account</label>
                        <br />
                 <small>We can help you to recover your password and security info. First, enter your registered username and email address</small>
                 <br />
                   <br />  
                    <div class="form-group">
                  
                    
         <asp:TextBox ID="txt_usname" runat="server" CssClass="input-material" ToolTip="Enter username here"></asp:TextBox>
                      <label for="register-username" class="label-material">User Name
                      <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_usname" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator></span>
                      </label>
                    </div>
                    <div class="form-group">
                  <asp:TextBox ID="txt_email" runat="server" CssClass="input-material" 
                            ToolTip="Enter email address here" TextMode="Email"></asp:TextBox>
                      <label for="register-email" class="label-material">Email Address 
                     <span><asp:RequiredFieldValidator ID="valdate" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_email" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator></span>
                           </label>
                    </div>
                 
                    <div class="form-group terms-conditions">
                     
                       <br />
                                    <asp:Label ID="lbl_mgs" runat="server" Font-Size="Small" 
                            ForeColor="#CCCCCC"></asp:Label>
              
                    </div>
                    <div class="form-group">
                
                  <asp:LinkButton ID="btn_recover" CssClass="btn btn-primary" runat="server" 
                            onclick="btn_recover_Click" ValidationGroup="add">Send</asp:LinkButton>

                    </div>
                  </div><small>Already have an account? </small><a href="Default.aspx" class="signup">Login</a>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="messagealert" id="alert_container" style="width: 500px"></div>
      </div>
           
      <div class="copyrights text-center">
     <p style="color: #008080">Develop by <a href="#" class="external"> atodacOne (cadzronz02@gmail.com)</a>
      
        </p>
      </div>
    </div>
    <!-- JavaScript files-->
    <script src="distribution/vendor/jquery/jquery.min.js"></script>
    <script src="distribution/vendor/popper.js/umd/popper.min.js"> </script>
    <script src="distribution/vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="distribution/vendor/jquery.cookie/jquery.cookie.js"> </script>
    <script src="distribution/vendor/chart.js/Chart.min.js"></script>
    <script src="distribution/vendor/jquery-validation/jquery.validate.min.js"></script>
    <!-- Main File-->
    <script src="distribution/js/front.js"></script>
    </form>
</body>
</html>


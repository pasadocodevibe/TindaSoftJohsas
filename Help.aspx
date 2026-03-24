<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <style type="text/css">
        .messagealert {
            width: 50%;
             top:75px;
            z-index: 1000;
            font-size: 15px;
            padding-left: 20px;
            position:fixed;
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
</asp:Content>
<asp:Content ID="Content2" runat="server" 
    contentplaceholderid="body">
      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">About Us    </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">About Us   </li>
             
            </ul>
         
          </div>
            <br />
             <div class="projects no-padding-top">
            <div class="container-fluid" >
                <div class="row">
                 <div class="col-lg-4">
                 </div>
         <div class="col-lg-4">
                  <div class="client card">
                    <div class="card-close">
                      <div class="dropdown">
                      
                      </div>
                    </div>
                    <div class="card-body text-center">
                      <div class="client-avatar"><img src="distribution/img/avataryblank.jpg" alt="..." class="img-fluid rounded-circle">
                        <div class="status bg-green"></div>
                      </div>
                      <div class="client-title">
                        <h3>Ronnie B. Cadorna Jr.</h3><span>Freelance Software Developer</span><a href="#">cadzronz02@gmail.com</a>
                        <hr />
                      <b><span class="fa fa-fb"></span> <a href="https://www.facebook.com/cadota.cadorna/"> Visit Facebook Account</a> </b>
                      </div>
                   
                  </div>
                </div>
          </div>
           <div class="col-lg-4">
                 </div>
                  <div class="messagealert" id="alert_container"></div> 
                  </div>
                  </div>
    </asp:Content>



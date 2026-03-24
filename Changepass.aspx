<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Changepass.aspx.cs" Inherits="Changepass" %>

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
              <h2 class="no-margin-bottom">Change password    </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Change password   </li>
             
            </ul>
         
          </div>
         <br />
          <div class="forms">
                       <asp:TextBox ID="get_oldpass" Text=""  Style="display:none;" runat="server" ReadOnly="True" Width="0px"></asp:TextBox>
              <asp:HiddenField ID="hd_cpass" runat="server" />
                 <asp:HiddenField ID="hd_id" runat="server" />
         <div class="container-fluid" >
              <div class="row">
                <!-- Basic Form-->
                <div class="col-lg-4">
                </div>
                <div class="col-lg-4">
                  <div class="card">
                    <div class="card-close">
                      <div class="dropdown">
                        <button type="button" id="closeCard1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                        <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                      </div>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4">Change Password</h3>
                    </div>
                    <div class="card-body">

                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                      <p>Please provide all necessary information. </p>
                     
                        <div class="form-group">
                          <label class="form-control-label">Current password
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_oldpass" SetFocusOnError="True"></asp:RequiredFieldValidator></span>
                          <span>  
             
                             <asp:CompareValidator ID="CompareValidator2" runat="server" 
                            ErrorMessage=" * Invalid old password!" ControlToCompare="get_oldpass" 
                            ControlToValidate="txt_oldpass" Font-Size="Small" ForeColor="#CC3300" 
                                SetFocusOnError="True"></asp:CompareValidator></span>
                          </label>
                         

                  
                            <asp:TextBox ID="txt_oldpass" placeholder="Current password"    TextMode="Password"  runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                     <div class="form-group">       
                          <label class="form-control-label">New password
                               <span><asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                              runat="server" ErrorMessage=" * required!" Font-Size="Small" 
                              ForeColor="#CC3300" ControlToValidate="txt_newpass" SetFocusOnError="True"></asp:RequiredFieldValidator></span>
                     
                          </label>
                              <asp:TextBox ID="txt_newpass" placeholder="New password" TextMode="Password"  runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                         <div class="form-group">       
                          <label class="form-control-label">Retype new password 
                               <span><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                 ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                 ControlToValidate="txt_newpassverify" SetFocusOnError="True"></asp:RequiredFieldValidator></span>
                          
                          <span>     <asp:CompareValidator ID="CompareValidator1" runat="server" 
                            ErrorMessage=" * Password mismatch!" ControlToCompare="txt_newpass" 
                            ControlToValidate="txt_newpassverify" Font-Size="Small" ForeColor="#CC3300" 
                                 SetFocusOnError="True"></asp:CompareValidator></span>
                       </label> 

                              <asp:TextBox ID="txt_newpassverify" placeholder="Retype new password" TextMode="Password"  runat="server" CssClass="form-control"></asp:TextBox>
                       
                       </div>
                    
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_changepass" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_changepass_Click">Change password</asp:LinkButton>
                      
                        </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                    </div>
                  </div>
                </div>
                 <div class="col-lg-4">
                </div>
                </div>
            
                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 
    </asp:Content>




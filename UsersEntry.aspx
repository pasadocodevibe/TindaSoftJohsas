<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UsersEntry.aspx.cs" Inherits="UsersEntry" %>


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
       <script type="text/javascript">
           function ShowPreview(input) {
               if (input.files && input.files[0]) {
                   var ImageDir = new FileReader();
                   ImageDir.onload = function (e) {
                       $('#body_impPrev').attr('src', e.target.result);
                   }
                   ImageDir.readAsDataURL(input.files[0]);
               }
           }  
    </script>  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">User Registration </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
                   <li class="breadcrumb-item"><a href="Users.aspx">User list</a></li>
              <li class="breadcrumb-item active">User Registration  </li>
             
            </ul>
         
          </div>
        <br />
          <div class="forms">
                     
               
         <div class="container-fluid" >
               <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" UpdateMode="Always">
                            <Triggers>
                <asp:PostBackTrigger ControlID="btn_add" />
            </Triggers>
                            <ContentTemplate>
                 
              <div class="row">

             
                              <asp:HiddenField ID="hd_id" runat="server" />
                                 <asp:HiddenField ID="hd_imgurl" runat="server" />
                <!-- Basic Form-->
                <div class="col-lg-6">
               <div class="card">
                    <div class="card-close">
                      <div class="dropdown">
                        <button type="button" id="closeCard1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                        <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                      </div>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4">
                          <asp:Label ID="lbl_title" runat="server" Text="User Registration"></asp:Label> </h3>
                    </div>
                    <div class="card-body">
                      <div class="card-body text-center">
                      <div class="client-avatar">
                      <asp:Image ID="impPrev" runat="server"  CssClass="img-fluid rounded-circle" ImageUrl="distribution/img/avataryblank.jpg" Height="150px" Width="150px" />

                    
                      </div>
                      
                    
                    </div>
                    <div class ="form-group">
                    <div class ="col-sm-12" align="center">
                    <asp:FileUpload ID="fileupload" CssClass="Form-control" runat="server" type="file" ToolTip="Browse your photo" onchange="ShowPreview(this)" Font-Size="Medium" />
                    <span>
                    <asp:LinkButton ID="btn_removeimg" CssClass="btn btn-danger btn-sm " onclick="btn_removeimg_Click" runat="server"
                      OnClientClick="return getConfirmation_verify(this, 'Please confirm','Are you sure you want to Delete image?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>
                       
                    
                        </span>
                        <p><asp:Label ID="lbl_uploadstat" runat="server" Text="" ForeColor="#FF3300" Font-Size="Small" Font-Italic="False"></asp:Label></p>
                    </div>
                    </div>

                       <div class="form-group">
                           
                           
                            <label class="form-control-label">Fullname
             <span><asp:RequiredFieldValidator ID="valdate" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_fname" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                
                                </span>
                        
                          </label>
                           
                     <asp:TextBox ID="txt_fname" placeholder="Fullname" runat="server" CssClass="form-control"></asp:TextBox>
                       
                      
                      
                        
                     
                         
                    
                        </div> 
                    

                     
                        <div class="form-group">
                            <div class="row">
                            <div class="col-sm-12">
                            <label class="form-control-label">Email
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_email" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator></span>
                        
                          </label>
                           
                     <asp:TextBox ID="txt_email" placeholder="Email address" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                             </div>
                       
                        </div>
                        </div>
                      
                        <div class="form-group">
                            <div class="row">
                            <div class="col-sm-6">
                            <label class="form-control-label">Username
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_usname" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                         <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage=" char. not allowed" 
                              ControlToValidate="txt_usname" Font-Size="Small" 
                              ForeColor="#CC3300"  ValidationExpression="^[a-zA-Z0-9]+$"  SetFocusOnError="True" ValidationGroup="add"></asp:RegularExpressionValidator>
                              </span>
                          </label>
                           
                     <asp:TextBox ID="txt_usname" placeholder="Username" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                        <div class="col-sm-6">
                          <label class="form-control-label">Password
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_psword" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator></span>
                        
                          </label>
                         <asp:TextBox ID="txt_psword" placeholder="Password" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                        
                        </div>
                        
                        </div>
                        </div>
                            <div class="form-group">
                             <div class="row">
                            
                                <div class="col-sm-6">
                          <label class="form-control-label">User Role
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="dp_aclevel" SetFocusOnError="True" ValidationGroup="add" InitialValue="Select..."></asp:RequiredFieldValidator></span>
                        
                          </label>
                         

                       <asp:DropDownList ID="dp_aclevel" CssClass="form-control" runat="server">
                                       </asp:DropDownList>
                             </div>
                             <div class="col-sm-6">
                          <label class="form-control-label">Status
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_active" SetFocusOnError="True" ValidationGroup="add" ></asp:RequiredFieldValidator></span>
                        
                          </label>
                         

                       <asp:DropDownList ID="txt_active" CssClass="form-control" runat="server">
                           <asp:ListItem Value="Active">Active</asp:ListItem>
                           <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                                       </asp:DropDownList>
                             </div>
                         </div>
                        </div>
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_add" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_add_Click" ValidationGroup="add">Submit</asp:LinkButton>
                      
                        </div>
                     
                    </div>
                  </div>
                </div>
            

                 





                <div class="messagealert" id="alert_container"></div>
                </div>
                </ContentTemplate>
                </asp:UpdatePanel>
                </div>
                 

</div>
</asp:Content>


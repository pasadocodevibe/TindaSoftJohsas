<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="GeneralSettings.aspx.cs" Inherits="GeneralSettings" %>

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
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">General Settings </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Setup / General Settings  </li>
             
            </ul>
         
          </div>
        <br />
          <div class="forms">
                     
             
         <div class="container-fluid" >
              <div class="row">
                <!-- Basic Form-->
                <div class="col-lg-12">
               <div class="card">
                    <div class="card-close">
                      <div class="dropdown">
                        <button type="button" id="closeCard1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                        <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                      </div>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4">Store Settings</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                             <div class="row">
                              <div class="col-6">

                                <asp:HiddenField ID="hd_id" runat="server" />
                   
                     
                        <div class="form-group">Store Name
                          <label class="form-control-label">
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_storename" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                 
                                </span>
                        
                          </label>
                            <asp:TextBox ID="txt_storename" placeholder="Store Name"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                         <div class="form-group">Store Address
                          <label class="form-control-label">
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_address" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                            <asp:TextBox ID="txt_address"  placeholder="Store Address"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                           <div class="form-group">Contact No.
                          <label class="form-control-label">
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_contact" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                            <asp:TextBox ID="txt_contact"  placeholder="Contact Number"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                    
                   </div>
                  
                  
                         <div class="col-6">
                     <div class="form-group">Store Email Address
                          <label class="form-control-label">
                          <span><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_email" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                          
                                </span>
                        
                          </label>
                            <asp:TextBox ID="txt_email"  placeholder="Email Address"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                        <div class="form-group">Store Details
                          <label class="form-control-label">
                              <span><asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_details" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>

                                </span>
                        
                          </label>
                            <asp:TextBox ID="txt_details"  TextMode="MultiLine" placeholder="Store Details"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                         <div class="form-group">Theme
                          <label class="form-control-label">
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_status" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                
                                </span>
                        
                          </label>
                         

                            <asp:DropDownList ID="dp_style" runat="server" CssClass="form-control">
                                <asp:ListItem Value="distribution/css/style.default.css">Default</asp:ListItem>
                                <asp:ListItem  Value="distribution/css/style.blue.css">Blue</asp:ListItem>
                                <asp:ListItem  Value="distribution/css/style.green.css">Green</asp:ListItem>
                                <asp:ListItem  Value="distribution/css/style.sea.css">Sea</asp:ListItem>
                                <asp:ListItem  Value="distribution/css/style.violet.css">Violet</asp:ListItem>
                                <asp:ListItem  Value="distribution/css/style.red.css">Red</asp:ListItem>
                                   <asp:ListItem  Value="distribution/css/style.pink.css">Pink</asp:ListItem>
                            </asp:DropDownList>
                        
           
                        </div>



                        <div class="form-group">Status
                          <label class="form-control-label">
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_status" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                   <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="* special char. not allowed" 
                              ControlToValidate="txt_status" Font-Size="Small" 
                              ForeColor="#CC3300"  ValidationExpression="^[a-zA-Z0-9\ \&]+$"  ValidationGroup="add" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                </span>
                        
                          </label>
                         

                            <asp:DropDownList ID="txt_status" runat="server" CssClass="form-control">
                                <asp:ListItem>Active</asp:ListItem>
                                <asp:ListItem>Inactive</asp:ListItem>
                            </asp:DropDownList>
                        
           
                        </div>
                         </div>
                         </div>
                    <div class="row pull-right">
                    <div class="col-12">
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_add" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_add_Click" ValidationGroup="add"><i class="fa fa-send"></i> Submit</asp:LinkButton>
                                 
                      
                        </div>
                        </div>
                      </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                    </div>
                  </div>
                </div>
            


            <div class="col-lg-12">
               <div class="card">
                    <div class="card-close">
                      <div class="dropdown">
                        <button type="button" id="Button1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                        <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                      </div>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4">Report Settings</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
                          <Triggers>
                <asp:PostBackTrigger ControlID="btn_reportsettingadd" />
            </Triggers>
                            <ContentTemplate>
                             <div class="row">
                              <div class="col-6">

                                <asp:HiddenField ID="hd_reportsettingsid" runat="server" />
                   
                        <div class="form-group">
                          <label class="form-control-label">Report Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_reportname" SetFocusOnError="True" InitialValue="Select..." ValidationGroup="add1"></asp:RequiredFieldValidator>
                                 
                                </span>
                        
                          </label>

                                <asp:DropDownList ID="txt_reportname" CssClass="form-control" runat="server">
                                </asp:DropDownList>
                          
           
                        </div>
                     
                        <div class="form-group">
                          <label class="form-control-label">Paper Size
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_papertype" SetFocusOnError="True" ValidationGroup="add1" InitialValue="Select..."></asp:RequiredFieldValidator>
                                 
                                </span>
                        
                          </label>
                             <asp:DropDownList ID="txt_papertype" CssClass="form-control" runat="server">
                                </asp:DropDownList>
           
                        </div>
                       
                          <div class="form-group">
                          <label class="form-control-label">Page Orientation
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_pageorientation" SetFocusOnError="True" ValidationGroup="add1"></asp:RequiredFieldValidator>
                                 
                                </span>
                        
                          </label>
                             <asp:DropDownList ID="txt_pageorientation" CssClass="form-control" runat="server">
                                 <asp:ListItem>Portrait</asp:ListItem>
                                 <asp:ListItem>Landscape</asp:ListItem>
                                </asp:DropDownList>
           
                        </div>
                 
              
                 
                       <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Page Margin
                           <span><asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                                ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_bleft" SetFocusOnError="True" ValidationGroup="add1"></asp:RequiredFieldValidator>
                                   <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"  ForeColor="#CC3300"
                               ErrorMessage="*"  Font-Size="Small" ValidationGroup="add1" ControlToValidate="txt_bleft" ValidationExpression="^(100|[1-9][0-9]?)$"></asp:RegularExpressionValidator>
                                
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_bright" SetFocusOnError="True" ValidationGroup="add1"></asp:RequiredFieldValidator>

                                     <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server"  ForeColor="#CC3300"
                               ErrorMessage="*"  Font-Size="Small" ValidationGroup="add1" ControlToValidate="txt_bright" ValidationExpression="^(100|[1-9][0-9]?)$"></asp:RegularExpressionValidator>
                                

                                  <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" 
                                ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_btop" SetFocusOnError="True" ValidationGroup="add1"></asp:RequiredFieldValidator>

                                     <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server"  ForeColor="#CC3300"
                               ErrorMessage="*"  Font-Size="Small" ValidationGroup="add1" ControlToValidate="txt_btop" ValidationExpression="^(100|[1-9][0-9]?)$"></asp:RegularExpressionValidator>
                                


                                  <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" 
                                ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_bbottom" SetFocusOnError="True" ValidationGroup="add1"></asp:RequiredFieldValidator>

                                     <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server"  ForeColor="#CC3300"
                               ErrorMessage="*"  Font-Size="Small" ValidationGroup="add1" ControlToValidate="txt_bbottom" ValidationExpression="^(100|[1-9][0-9]?)$"></asp:RegularExpressionValidator>
                                
                                </span>
                          
                          </label>


                          <div class="col-sm-9">
                            <div class="row">
                              <div class="col-md-3">
                               <asp:TextBox ID="txt_bleft" ToolTip="Left Margin"  placeholder="Left Margin" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                              </div>
                              <div class="col-md-3">
                                   <asp:TextBox ID="txt_bright" ToolTip="Right Margin"  placeholder="Right Margin" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                              </div>
                              <div class="col-md-3">
                                    <asp:TextBox ID="txt_btop" ToolTip="Top Margin" placeholder="Top Margin" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                              </div>
                              <div class="col-md-3">
                                   <asp:TextBox ID="txt_bbottom" ToolTip="Bottom Margin" placeholder="Bottom Margin" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                              </div>
                            </div>
                          </div>
                        </div>
                   
                              <div class="form-group">
                          <label class="form-control-label">Report Header - Left Logo
            
                          </label>
                          
                                  <asp:FileUpload ID="leftlogo" CssClass="form-control" runat="server" />
                        </div>
                          <div class="form-group">
                          <label class="form-control-label">Report Header - Right Logo
            
                          </label>
                          
                                  <asp:FileUpload ID="rightlogo" CssClass="form-control" runat="server" />
                        </div>
                   </div>
                  
                  
                       <div class="col-lg-6">
              <asp:UpdatePanel ID="UpdatePanel2" runat="server" >
                            <ContentTemplate>
                  <div class="card">
                    <div class="card-close">
                     
                                                
                                                <span class="profile-ava pull-right">
                                                 <div class="col-lg-12"> 
                                                <div class="form-group">
                              <div class="input-group">
                               


                                <asp:TextBox ID="txt_search" CssClass="form-control" runat="server" 
                                                        placeholder="Type Keywords Here..." AutoPostBack="True" ontextchanged="txt_search_TextChanged" 
                                                           ></asp:TextBox>
                                <div class="input-group-append">
                                           
                            <asp:LinkButton ID="btn_search"  CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_search_Click"><i class="fa fa-search"></i> </asp:LinkButton>
                                        
                                   
                                </div>
                              </div>
                            </div>
                                                            </div>
                                                              
                                                </span> 
                                                    
                                          
                                             
                    </div>
                    <div class="card-header d-flex align-items-center"  style="height: 65px">
                      <h3 class="h4">List</h3>
                    </div>
                    <div class="card-body">
                      




                      <div class="table-responsive">   


                      <asp:GridView ID="gv_branch" runat="server" 
                                                  CssClass="table  table-sm table-hover" 
                                                  AutoGenerateColumns="false" AllowPaging="true"
                                            OnPageIndexChanging="OnPaging" onrowdatabound="gv_branch_RowDataBound" GridLines="None" PagerSettings-Mode="NumericFirstLast" PageSize="10">
                                            <Columns>
                                             <asp:TemplateField ItemStyle-HorizontalAlign="NotSet" ItemStyle-Width="100px">
            <ItemTemplate>
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("reportid") %>' runat="server"></asp:HiddenField>
                   
                 
                <asp:LinkButton ID="btn_selectbranch" CssClass="btn btn-primary btn-sm " onclick="btn_selectbranch_Click" runat="server" ><i class="fa fa-edit"></i></asp:LinkButton>
                      <asp:LinkButton ID="btn_deletebranch" CssClass="btn btn-danger btn-sm " onclick="btn_deletebranch_Click" runat="server"
                      OnClientClick="return getConfirmation_verify(this, 'Please confirm','Are you sure you want to Delete?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
                                                <asp:BoundField DataField="rpname" HeaderText="Report Name" ItemStyle-HorizontalAlign="Left" />
                                                  <asp:BoundField DataField="psizeorientation" HeaderText="Page Size" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="Margin" HeaderText="Margin" ItemStyle-HorizontalAlign="Left" />
                                               
                                              
                                                  
                                            </Columns>
                                        </asp:GridView>
                <asp:Label ID="lbl_item" runat="server" Text="" CssClass="form-control-label"></asp:Label>
                   
                      </div>

                    </div>
                  </div>


                  
                      </ContentTemplate>
                      </asp:UpdatePanel>
                </div>
                         </div>
                    <div class="row pull-right">
                    <div class="col-12">
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_reportsettingadd" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_reportsettingadd_Click" ValidationGroup="add1"><i class="fa fa-send"></i> Submit</asp:LinkButton>
                                 
                      
                        </div>
                        </div>
                      </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                    </div>
                  </div>
                </div>








                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 

      </div>
</asp:Content>



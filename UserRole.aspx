<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UserRole.aspx.cs" Inherits="UserRole" %>

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
              <h2 class="no-margin-bottom">User Role </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              
              <li class="breadcrumb-item active">User Role  </li>
             
            </ul>
         
          </div>
        <br />
          <div class="forms">
                     
               
         <div class="container-fluid" >
               <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" UpdateMode="Always">
                         
                            <ContentTemplate>
                 
              <div class="row">

             
                              <asp:HiddenField ID="hd_id" runat="server" />
                                
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
                          <asp:Label ID="lbl_title" runat="server" Text="Role & Permission"></asp:Label> </h3>
                    </div>
                    <div class="card-body">
            
                <div class="form-group">
                         <div class="row">
                           <div class="col-sm-6">
                           <label class="form-control-label">Role Name
             <span><asp:RequiredFieldValidator ID="valdate" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_rolename" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                
                                </span>
                        
                          </label>
                           
                     <asp:TextBox ID="txt_rolename" placeholder="Role Name" runat="server" CssClass="form-control"></asp:TextBox>  
                     
                     </div>
                     
                     
                     <div class="col-sm-6">
                          <label class="form-control-label">Status
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_active" SetFocusOnError="True" ValidationGroup="add" ></asp:RequiredFieldValidator></span>
                        
                          </label>
                         

                       <asp:DropDownList ID="txt_active" CssClass="form-control" runat="server">
                           <asp:ListItem Value="Active">Active</asp:ListItem>
                           <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                                       </asp:DropDownList>
                             </div>                       
                    </div>
                        </div>
                        <hr />
                        <div class="table-responsive  table-sm table-hover">
                     <h4>Role Permission</h4>
                   

                      <asp:GridView ID="gv_masterlist" runat="server" 
                                                  CssClass="table table-hover" 
                                                  AutoGenerateColumns="false" 
                                           
                              onrowdatabound="gv_masterlist_RowDataBound" GridLines="None" PagerSettings-Mode="NumericFirstLast">
                                            <Columns>
                                             <asp:TemplateField>
                                            <ItemTemplate>
                                             <asp:HiddenField ID="hd_permissionid" Value='<%#Eval("permissionid") %>' runat="server"></asp:HiddenField>
                                                    <asp:HiddenField ID="hd_sitename" Value='<%#Eval("psitename") %>' runat="server"></asp:HiddenField>
                                                       <asp:HiddenField ID="hd_menu" Value='<%#Eval("mainmenu") %>' runat="server"></asp:HiddenField>
                                                         <asp:HiddenField ID="hd_roleid" Value='<%#Eval("proleid") %>' runat="server"></asp:HiddenField>
                                                       <asp:HiddenField ID="hd_add" Value='<%#Eval("padd") %>' runat="server"></asp:HiddenField>
                                                       <asp:HiddenField ID="hd_delete" Value='<%#Eval("pdelete") %>' runat="server"></asp:HiddenField>
                                                       <asp:HiddenField ID="hd_edit" Value='<%#Eval("pedit") %>' runat="server"></asp:HiddenField>
                                                       <asp:HiddenField ID="hd_view" Value='<%#Eval("pview") %>' runat="server"></asp:HiddenField>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="mainmenu" HeaderText="Main Menu"    ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="psitename" HeaderText="Page Name"    ItemStyle-HorizontalAlign="Left" />
                                         
                                                 
                                                          <asp:TemplateField>
                                                           <HeaderTemplate>
                                                            Add
                                                            </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="checkadd" runat="server"  />
                                                                 </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                          <asp:TemplateField>
                                                           <HeaderTemplate>
                                                            Edit
                                                            </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="checkedit" runat="server"  />
                                                                 </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                          <asp:TemplateField>
                                                           <HeaderTemplate>
                                                            Delete
                                                            </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="checkdelete" runat="server"  />
                                                                 </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                          <asp:TemplateField>
                                                           <HeaderTemplate>
                                                           View
                                                            </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="checkview" runat="server"  />
                                                                 </ItemTemplate>
                                                            </asp:TemplateField>
                                                            
                                                         
                                                             
                                            </Columns>
                                        </asp:GridView>
                                         






                        
                      </div>

                            
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_add" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_add_Click" ValidationGroup="add">Submit</asp:LinkButton>
                      
                        </div>
                     
                    </div>
                  </div>
                </div>
            

                 <div class="col-lg-6">
                     <div class="card">
                        <div class="card-close">
                          <div class="dropdown">
                            <button type="button" id="Button1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                            <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                          </div>
                        </div>
                        <div class="card-header d-flex align-items-center">
                          <h3 class="h4">List of Role</h3>
                        </div>
                       <div class="card-body">
                      <div class="table-responsive  table-sm table-hover">




                      <asp:GridView ID="GridView_role" runat="server" 
                                                  CssClass="table table-hover" 
                                                  AutoGenerateColumns="false" AllowPaging="true"
                                            OnPageIndexChanging="OnPaging" PageSize="10" 
                              onrowdatabound="GridView_role_RowDataBound" GridLines="None" PagerSettings-Mode="NumericFirstLast">
                                            <Columns>
                                             <asp:TemplateField>
            <ItemTemplate>
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("levelid") %>' runat="server"></asp:HiddenField>
                     <asp:HiddenField ID="hd_rolename" Value='<%#Eval("levelname") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_status" Value='<%#Eval("lstatus") %>' runat="server"></asp:HiddenField>
                <asp:HiddenField ID="hd_setid" Value='<%#Eval("lsetid") %>' runat="server"></asp:HiddenField>
                <asp:LinkButton ID="btn_selectbranch" CssClass="btn btn-primary btn-sm " onclick="btn_selectbranch_Click" runat="server" ><i class="fa fa-edit"></i></asp:LinkButton>
                      <asp:LinkButton ID="btn_deletebranch" CssClass="btn btn-danger btn-sm " onclick="btn_deletebranch_Click" runat="server"
                      OnClientClick="return getConfirmation_verify(this, 'Please confirm','Are you sure you want to Delete?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
           <asp:TemplateField>
           <HeaderTemplate>
           #
           </HeaderTemplate>
            <ItemTemplate>
       
                <asp:Label ID="lbl_no" runat="server" Text="<%# Container.DataItemIndex + 1 %>  "></asp:Label>
             </ItemTemplate>
        </asp:TemplateField>
                                                <asp:BoundField DataField="levelname" HeaderText="Role Name" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="lstatus" HeaderText="Status" ItemStyle-HorizontalAlign="Left" />
                                                
                                               
                                                       
                                            </Columns>
                                        </asp:GridView>

                                         <asp:Label ID="lbl_item" runat="server" Text="" CssClass="form-control-label"></asp:Label>





                        
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
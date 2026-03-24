<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Users" %>

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
                $("#alert_div").fadeTo(3000, 500).slideUp(500, function () {
                    $("#alert_div").remove();
                });
            }, 1000); //5000=5 seconds
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Users</h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Setup / Users   </li>
        
            </ul>
                 
                 
                 
          </div>
        
          <div class="tables">
             
         <div class="container-fluid" >
         <br />
             
             <div class="row">
             <div class="col-lg-12">
                  <div class="card">
                   <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server">
                            <ContentTemplate>
                            
                    <div class="card-close">
                     
                                               
                                             
                                      
                                          
                                             
                    </div>
                    <div class="card-header d-flex align-items-center" style="height: 65px">
                      
                       
                            <asp:HiddenField ID="hd_id" runat="server" />
                             <div class="col-lg-2"> 
                      <asp:LinkButton  CssClass="btn btn-primary" ID="btn_additem" 
                      runat="server" PostBackUrl="~/UsersEntry.aspx">Add User Account</asp:LinkButton>
                                           </div>   
           <div class="col-lg-10"> 
                                                <div class="form-group" style="margin: 5px 0px 5px 0px">
                              <div class="input-group">
                             
                                                       
                      


                                <asp:TextBox ID="txt_search" CssClass="form-control" runat="server" 
                                                        placeholder="Type Keywords Here..." AutoPostBack="True" ontextchanged="txt_search_TextChanged" 
                                                           ></asp:TextBox>
                                <div class="input-group-append">
                                           
                            <asp:LinkButton ID="btn_search"  CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_search_Click"><i class="fa fa-search"></i> </asp:LinkButton>
                                         <asp:LinkButton ID="btn_reset"  CssClass="btn btn-secondary" runat="server" 
                                        onclick="btn_reset_Click"><i class="fa fa-refresh"></i> </asp:LinkButton>
                                   
                                </div>
                              </div>
                            </div>
                                                            </div>
                                                              


                    </div>
                    <div class="card-body">
                      <div class="table-responsive  table-sm table-hover">




                      <asp:GridView ID="gv_masterlist" runat="server" 
                                                  CssClass="table table-hover" 
                                                  AutoGenerateColumns="false" AllowPaging="true"
                                            OnPageIndexChanging="OnPaging" PageSize="10" 
                              onrowdatabound="gv_masterlist_RowDataBound" GridLines="None" PagerSettings-Mode="NumericFirstLast">
                                            <Columns>
                                             <asp:TemplateField>
            <ItemTemplate>
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("uid") %>' runat="server"></asp:HiddenField>
                     <asp:HiddenField ID="hd_usname" Value='<%#Eval("usname") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_actstatus" Value='<%#Eval("ustatus") %>' runat="server"></asp:HiddenField>
                <asp:HiddenField ID="hd_roleid" Value='<%#Eval("uroleid") %>' runat="server"></asp:HiddenField>
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
                                                <asp:BoundField DataField="usname" HeaderText="Username" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="ufullname" HeaderText="Fullname" ItemStyle-HorizontalAlign="Left" />
                                                
                                                <asp:BoundField DataField="role" HeaderText="Role"    ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="uemail" HeaderText="Email" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="ustatus" HeaderText="Status" ItemStyle-HorizontalAlign="Left" />
                                                  <asp:BoundField DataField="uregisterdate" HeaderText="Date Registered" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:MMM. dd, yyyy}" />
                                                 <asp:BoundField DataField="ulastlog" HeaderText="Last log" ItemStyle-HorizontalAlign="Left" />
                                                       
                                            </Columns>
                                        </asp:GridView>

                                         <asp:Label ID="lbl_item" runat="server" Text="" CssClass="form-control-label"></asp:Label>





                        
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

</asp:Content>


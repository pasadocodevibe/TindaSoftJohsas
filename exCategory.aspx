<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="exCategory.aspx.cs" Inherits="exCategory" %>

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
              <h2 class="no-margin-bottom">Expense Category </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active"> Expense Category  </li>
             
            </ul>
         
          </div>
        <br />
          <div class="forms">
                     
             
         <div class="container-fluid" >
              <div class="row">
                <!-- Basic Form-->
                <div class="col-lg-5">
               <div class="card">
                    <div class="card-close">
                      <div class="dropdown">
                        <button type="button" id="closeCard1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" class="dropdown-toggle"><i class="fa fa-ellipsis-v"></i></button>
                        <div aria-labelledby="closeCard1" class="dropdown-menu dropdown-menu-right has-shadow"><a href="#" class="dropdown-item remove"> <i class="fa fa-times"></i>Close</a></div>
                      </div>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4">Expense Category Entry</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                                <asp:HiddenField ID="hd_id" runat="server" />
                   
                     
                        <div class="form-group">
                          <label class="form-control-label">Category Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_brandname" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                   <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="* special char. not allowed" 
                              ControlToValidate="txt_brandname" Font-Size="Small" 
                              ForeColor="#CC3300"  ValidationExpression="^[a-zA-Z0-9\ \&]+$"  ValidationGroup="add" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                </span>
                        
                          </label>
                         

                  
                            <asp:TextBox ID="txt_brandname" placeholder="Category Name"    runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                    <div class="form-group">
                          <label class="form-control-label">Status
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
                   
                       
                    
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_add" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_add_Click" ValidationGroup="add">Submit</asp:LinkButton>
                                  <asp:LinkButton ID="btn_reset" CssClass="btn btn-default" runat="server" 
                                onclick="btn_reset_Click" >Reset</asp:LinkButton>
                      
                        </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                    </div>
                  </div>
                </div>
            


            <div class="col-lg-7">
              <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
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
                      <h3 class="h4">List of Expense Category</h3>
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
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("excategid") %>' runat="server"></asp:HiddenField>
                   
                      <asp:HiddenField ID="HiddenField1" Value='<%#Eval("excategoryname") %>' runat="server"></asp:HiddenField>
                       
                      <asp:HiddenField ID="hd_status" Value='<%#Eval("excategstatus") %>' runat="server"></asp:HiddenField>
                <asp:LinkButton ID="btn_selectbranch" CssClass="btn btn-primary btn-sm " onclick="btn_selectbranch_Click" runat="server" ><i class="fa fa-edit"></i></asp:LinkButton>
                      <asp:LinkButton ID="btn_deletebranch" CssClass="btn btn-danger btn-sm " onclick="btn_deletebranch_Click" runat="server"
                      OnClientClick="return getConfirmation_verify(this, 'Please confirm','Are you sure you want to Delete?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
                                                <asp:BoundField DataField="excategoryname" HeaderText="Category Name" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="excategstatus" HeaderText="Status" ItemStyle-HorizontalAlign="Left" />
                                              
                                                  
                                            </Columns>
                                        </asp:GridView>
                <asp:Label ID="lbl_item" runat="server" Text="" CssClass="form-control-label"></asp:Label>
                   
                      </div>

                    </div>
                  </div>


                  
                      </ContentTemplate>
                      </asp:UpdatePanel>
                </div>








                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 

      </div>
</asp:Content>





<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="mStockSheet.aspx.cs" Inherits="mStockSheet" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
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
              <h2 class="no-margin-bottom">Stock Sheet</h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
                    <li class="breadcrumb-item">Inventory  </li>
              <li class="breadcrumb-item active">Stock Sheet  </li>
        
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
                  
                                           </div>   
           <div class="col-lg-10"> 
                                                <div class="form-group" style="margin: 5px 0px 5px 0px">
                              <div class="input-group">
                             
                                                       
                      


                                <asp:TextBox ID="txt_search" CssClass="form-control" runat="server" 
                                                        placeholder="Type Keywords Here..." AutoPostBack="True" ontextchanged="txt_search_TextChanged" 
                                                           ></asp:TextBox>
                                <div class="input-group-append">
                                <asp:DropDownList ID="txt_searchcategory" CssClass="form-control"  runat="server">
                                                               </asp:DropDownList>
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
                       <asp:HiddenField ID="hd_productid" runat="server" Value='<%# Eval("productid") %>'></asp:HiddenField>
          
                 
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
                                                <asp:BoundField DataField="productname" HeaderText="Item Name"  ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="baseunit" HeaderText="Base Unit"  ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="category" HeaderText="Category" ItemStyle-HorizontalAlign="Left" />
                                                
                                                <asp:BoundField DataField="in" HeaderText="In" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="out" HeaderText="Out"  ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="stockqty" HeaderText="Running Stock" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:TemplateField>
                                                <ItemTemplate>
                                                                           <asp:Panel ID="Panel_col" runat="server">
                                                       <asp:Label ID="lbl_noofaging" runat="server" Text=""></asp:Label> <asp:LinkButton ID="lnkbutton" runat="server"></asp:LinkButton>
                                                     </asp:Panel>                                        <cc1:CollapsiblePanelExtender runat="server" ID="cpe" TargetControlID="Panel_content" CollapseControlID="Panel_col" ExpandControlID="Panel_col" Collapsed="true" CollapsedSize="0" ExpandedText="Hide list" CollapsedText="Show list" TextLabelID="lnkbutton" ScrollContents="True" SuppressPostBack="True">
    </cc1:CollapsiblePanelExtender>                 


                       <tr>
                                           
                                             
                                             <td colspan="9">
                                             <asp:Panel ID="Panel_content" runat="server" CssClass="table-responsive" ScrollBars="Both">      
                
                     
                            <table class="table table-bordered">
                              <thead>
                                <tr>
                                
                                  <th>Item Name</th>
                                  <th>Base Unit</th>
                                  <th>Category</th>
                                  <th>Type</th>
                                  <th>Quantity</th>
                                     <th>Cost per unit</th>
                                        <th>Note</th>
                                          <th>Date Entry</th>
                                       
                                </tr>
                              </thead>
                              <tbody>
                                 <asp:Repeater ID="Repeater_aging" runat="server"  EnableTheming="true">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                               <ItemTemplate>
                                                    <tr>
                                            <asp:HiddenField ID="hd_inventoryid" runat="server" Value='<%# Eval("inventoryid") %>'></asp:HiddenField>
                                            <asp:HiddenField ID="hd_inproductid" runat="server" Value='<%# Eval("inproductid") %>'></asp:HiddenField>
                                       
                                                    
                                                    
                                                      <td><asp:Label ID="Label9" runat="server" Text='<%# Eval("name") %>'/></td>
                                                      <td><asp:Label ID="Label10" runat="server" Text='<%# Eval("unit") %>'/></td>
                                                      <td><asp:Label ID="Label11" runat="server" Text='<%# Eval("category") %>'/></td>
                                                      <td><asp:Label ID="Label12" runat="server" Text='<%# Eval("inventorytype") %>'/></td>
                                                      <td><asp:Label ID="Label13" runat="server" Text='<%# Eval("abqty","{0:N}") %>'/></td>
                                                           <td><asp:Label ID="Label15" runat="server" Text='<%# Eval("inventorycostperunit","{0:N}") %>'/></td>
                                                      <td><asp:Label ID="Label14" runat="server" Text='<%# Eval("inventorynote") %>'/></td>
                                                       <td><asp:Label ID="Label1" runat="server" Text='<%# Eval("inventorydate") %>'/></td>
                                                    </tr>
                                   </ItemTemplate>
                                     <FooterTemplate>
                                     
                                     </FooterTemplate>
                                  </asp:Repeater> 
                               
                              
                              </tbody>
                            </table>

            </asp:Panel>
                                             </td>
                                             </tr>
                                                </ItemTemplate>
                                                </asp:TemplateField>
                                                       
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


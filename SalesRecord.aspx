<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SalesRecord.aspx.cs" Inherits="SalesRecord" %>
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
      <script type="text/javascript">
             function openModalsearch() {
                 $('#myModal_search').modal('show');
             } </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Sales Record</h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Sales Record   </li>
        
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
                    <span>
                         <asp:LinkButton  CssClass="btn btn-success" ID="btn_additem" 
                      runat="server" PostBackUrl="~/pos1.aspx" TabIndex="1" ToolTip="Shortcut key: alt+A" AccessKey="A"><i class="fa fa-plus"></i></asp:LinkButton>


                                          <asp:LinkButton ID="btn_search"  TabIndex="2" ToolTip="Shortcut key: alt+S" AccessKey="S" CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_search_Click"><i class="fa fa-search"></i> </asp:LinkButton>
                                         <asp:LinkButton ID="btn_reset" TabIndex="3" ToolTip="Shortcut key: alt+R" AccessKey="R" CssClass="btn btn-secondary" runat="server" 
                                        onclick="btn_reset_Click"><i class="fa fa-refresh"></i> </asp:LinkButton>      
                            </span>
                     
                    </div>              
                                             

                    <div class="card-header d-flex align-items-center" style="height: 65px">
                      
                       
                            <asp:HiddenField ID="hd_id" runat="server" />
                          
          

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
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("invoiceid") %>' runat="server"></asp:HiddenField>
                         <asp:HiddenField ID="hd_name" Value='<%#Eval("invoiceno") %>' runat="server"></asp:HiddenField>
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
                                                <asp:BoundField DataField="invoicedate" HeaderText="Date Invoice"  DataFormatString="{0:MMM. dd, yyyy}" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="invoiceno" HeaderText="Invoice No." ItemStyle-HorizontalAlign="Left" />
                                              
                                                 <asp:BoundField DataField="invoicecustomer" HeaderText="Customer" ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="invoicesubtotal" HeaderText="Sub total" DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                            <asp:BoundField DataField="invoicediscountamt" HeaderText="Discount" DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                   
                                            <asp:BoundField DataField="invoicetax" HeaderText="Tax" DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                                  <asp:BoundField DataField="invoicetotal" HeaderText="Grand Total " DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="invoinceamounttendered" HeaderText="Amount Tendered" DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                                         <asp:BoundField DataField="invoicechanged" HeaderText="Change" DataFormatString="{0:N}"   ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="invoicenote" HeaderText="Note" ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="entryby" HeaderText="Entry by" ItemStyle-HorizontalAlign="Left" />
                                                       
                                                       <asp:TemplateField>
                                                <ItemTemplate>
                                                                           <asp:Panel ID="Panel_col" runat="server">
                                                       <asp:Label ID="lbl_noofaging" runat="server" Text=""></asp:Label> <asp:LinkButton ID="lnkbutton" runat="server"></asp:LinkButton>
                                                     </asp:Panel>                                        <cc1:CollapsiblePanelExtender runat="server" ID="cpe" TargetControlID="Panel_content" CollapseControlID="Panel_col" ExpandControlID="Panel_col" Collapsed="true" CollapsedSize="0" ExpandedText="Hide cart" CollapsedText="Show cart" TextLabelID="lnkbutton" ScrollContents="True" SuppressPostBack="True">
    </cc1:CollapsiblePanelExtender>                 


                       <tr>
                                           
                                             <td></td>
                                                       <td></td>
                                                                 <td></td>
     <td></td>
                                                                      <td></td>
                                             <td colspan="12">
                                             <asp:Panel ID="Panel_content" runat="server" CssClass="table-responsive" ScrollBars="Both">      
                
                     
                            <table class="table table-bordered">
                              <thead>
                                <tr>
                                
                                  <th>Item Name</th>
                                  <th>Price</th>
                                  <th>Quantity</th>
                                  <th>Amount</th>
                                  <th>Date Entry</th>
                                 
                                       
                                </tr>
                              </thead>
                              <tbody>
                                 <asp:Repeater ID="Repeater_cart" runat="server"  EnableTheming="true">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                               <ItemTemplate>
                                                    <tr>
                                           
                                                    
                                                    
                                                      <td><asp:Label ID="Label9" runat="server" Text='<%# Eval("itemname") %>'/></td>
                                                      <td><asp:Label ID="Label10" runat="server" Text='<%# Eval("price","{0:N}") %>'/></td>
                                                      <td><asp:Label ID="Label11" runat="server" Text='<%# Eval("qty") %>'/></td>
                                                      <td><asp:Label ID="Label12" runat="server" Text='<%# Eval("amount","{0:N}") %>'/></td>
                                                      <td><asp:Label ID="Label13" runat="server" Text='<%# Eval("cartdatecreated") %>'/></td>
                                                       
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




                 <div class="modal fade" id="myModal_search"  role="dialog" >
                 <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                          
                                            <h4 class="modal-title">
                                                <span id="Span1">
                                               Search
                                                </span>
                                            </h4>
                                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                            
                         <div class ="col-sm-12">

                         
                        <div class="form-group row">
                         
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                                         <span class="input-group-text">Date Filter 
                                          
                                         </span> 

                            <asp:DropDownList ID="dp_filterby"  CssClass="form-control" runat="server" 
                                    AutoPostBack="True" onselectedindexchanged="dp_daterange_SelectedIndexChanged" TabIndex="4">

                            </asp:DropDownList>
                       
                            </div>
                            </div>
                        </div>
                      

                          <div class="form-group row">
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                       
                              <asp:TextBox ID="txt_datefrom"  TabIndex="5" placeholder="Date From" TextMode="Date"  runat="server" CssClass="form-control" ToolTip="Filter by date from" Enabled="False"></asp:TextBox>
                           
                            <div class="input-group-append">
                                <span class="input-group-text">To </span> 
                             <asp:TextBox ID="txt_dateto"  TabIndex="6" placeholder="Date To" TextMode="Date"  runat="server" CssClass="form-control"  ToolTip="Filter by date to" Enabled="False"></asp:TextBox>
                            </div>
                              </div>
                            </div>
                         </div>
                         

                        <div class="form-group row">
                         
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                     
                            <asp:TextBox ID="txt_searchkeyword" TabIndex="7" placeholder="Enter Keywords" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            </div>
                        </div>
                      
                         </div>
                          

                              </ContentTemplate>
                          <Triggers>

                <asp:AsyncPostBackTrigger ControlID ="btn_adsearch"  />
                </Triggers>
            </asp:UpdatePanel>


                        </div>
                                        <div class="modal-footer">
                                          
                                       
                                         <asp:LinkButton ID="btn_adsearch" TabIndex="8" ToolTip="Shortcut key: alt+V" AccessKey="V"  CssClass="btn btn-primary"
                            runat="server"   onclick="btn_adsearch_Click"  ValidationGroup="search"
                                   ><i class="fa fa-search"></i> Find </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                </div>
  
     




                <div class="messagealert" id="alert_container"></div>
      

                
                </div>
                 </div>

</asp:Content>







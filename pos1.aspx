<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="pos1.aspx.cs" Inherits="pos1" %>
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
        .rbl input[type="radio"]
{
   margin-left: 3px;
   margin-right: 3px;
}
.rbl label
{
  margin-right: 8px;
  
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
                $("#alert_div").fadeTo(5000, 500).slideUp(500, function () {
                    $("#alert_div").remove();
                });
            }, 1000); //5000=5 seconds
        }
    </script>
       <script type="text/javascript">
          
           function getConfirmation_addcustomer(sender, title, message) {
               $("#spnTitle7").text(title);

               $('#modalpopup_customer').modal('show');
               $('#btnConfirm7').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
               return false;
           }
           function getConfirmation_verifysale(sender, title, message) {
               $("#spnTitle3").text(title);
               $("#spnMsg3").text(message);
               $('#modalPopUp_completesale').modal('show');
               $('#btnConfirm3').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
               return false;
           }
           function getConfirmation_verifys(sender, title, message) {
               $("#spnTitle33").text(title);
               $("#spnMsg33").text(message);
               $('#modalPopUp_Delete2').modal('show');
               $('#btnConfirm33').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
               return false;
           }
    </script>
     <script type="text/javascript">
         function openModal() {
          
             $('#myModal_receipt').modal('show');
            
         }
         function openModal_record() {

             $('#myModal_customersoa').modal('show');
         }
    </script>
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Point of Sale </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Point of Sale </li>
             
            </ul>
         
          </div>
     
          <div class="tables">
                     
                <br />
         <div class="container-fluid" >
              <div class="row">
                <!-- Basic Form-->
                <div class="col-lg-12">
               <div class="card">
                    <div class="card-close">
                    <span>
                                             <asp:LinkButton ID="btn_createtransact" TabIndex="8"  CssClass="btn btn-primary"
                            runat="server"   AccessKey="N" onclick="btn_createtransact_Click" ToolTip="New sale (shortcut: alt+N) "><i class="fa fa-plus"></i></asp:LinkButton>
                                     <asp:LinkButton ID="btn_retreive" AccessKey="P" TabIndex="9"  CssClass="btn btn-primary"
                            runat="server"  onclick="btn_retreive_Click" ToolTip="Print preview (shortcut: alt+P) "><i class="fa fa-print"></i></asp:LinkButton>
                            </span>
                     
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4"> Point of Sale  &nbsp;&nbsp;</h3>
                         <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
                         
                            <ContentTemplate>
                        <asp:Label ID="lbl_cartcount" runat="server" Text=""></asp:Label>  
                        </ContentTemplate> 
                    </asp:UpdatePanel>
                  
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                         
                            <ContentTemplate>
                                 



                                <asp:HiddenField ID="hd_id" runat="server" />
                                 <asp:HiddenField ID="hd_tobedeleted" runat="server" />
                                
                   <div class="row">
                   
                         <div class="col-lg-7">
                         
                            <div class="input-group">
                                         <cc1:AutoCompleteExtender ServiceMethod="Searchcname"
                                                    MinimumPrefixLength="2"
                                                    CompletionInterval="100" EnableCaching="False" CompletionSetCount="5"
                                                    TargetControlID="txt_searchitem"
                                                    ID="AutoCompleteExtender2" runat="server" 
                                       FirstRowSelected = "false" CompletionListCssClass="completionList_default"
                                                                                      CompletionListHighlightedItemCssClass="itemHighlighted"                        
                                       CompletionListItemCssClass="listItem" ServicePath="pos1.aspx">
                                                </cc1:AutoCompleteExtender>



                                <asp:TextBox ID="txt_searchitem" CssClass="form-control" runat="server" 
                                                        placeholder="Search item description/barcode" TabIndex="1" AccessKey="1" ToolTip="Search item (focus shortcut: alt+1)"></asp:TextBox>
                                <div class="input-group-append">
                                            <asp:TextBox ID="txt_itemqty" CssClass="form-control" runat="server" 
                                                        placeholder="Quantity" AutoPostBack="True" 
                                             ontextchanged="txt_itemqty_TextChanged" TextMode="Number" AccessKey="2" TabIndex="2" ToolTip="Enter quantity"></asp:TextBox>
                                                          
                    
                     
                                   







                                </div>
                              </div>
                      <br />
                                <div class="table-responsive">

                                 <asp:GridView ID="GridView1" runat="server" GridLines="None" 
                                        CssClass="table table-sm table-hover"  AutoGenerateColumns="false" 
                                        onrowdeleting="GridView1_RowDeleting" 
                                        onrowupdating="GridView1_RowUpdating" >
                            <Columns>
                            <asp:TemplateField>
                           <HeaderTemplate># </HeaderTemplate>
                            <ItemTemplate>
                             <asp:HiddenField ID="hd_cartdateadded" Value='<%#Eval("cartdatecreated") %>' runat="server"></asp:HiddenField>
                               <asp:HiddenField ID="hd_cart" Value='<%#Eval("cartid") %>' runat="server"></asp:HiddenField>
                                <asp:HiddenField ID="hd_itemname" Value='<%#Eval("itemname") %>' runat="server"></asp:HiddenField>
                                       <asp:Label ID="lbl_no" runat="server" Text="<%# Container.DataItemIndex + 1 %>  "></asp:Label>
                          <asp:HiddenField ID="hd_id" Value='<%#Eval("id") %>' runat="server"></asp:HiddenField>
                             </ItemTemplate>   </asp:TemplateField>

                                <asp:BoundField DataField="itemname" HeaderText="Items"  />
                                  <asp:TemplateField>
                           <HeaderTemplate>Price </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txt_price" CssClass="form-control"  AutoPostBack="True" 
                                             ontextchanged="txt_price_TextChanged" TextMode="Number" Text='<%# Eval("price") %>' runat="server"  Width="120px" AccessKey="*"></asp:TextBox>
                             </ItemTemplate>   </asp:TemplateField>
                               <asp:TemplateField>
                           <HeaderTemplate>Qty </HeaderTemplate>
                            <ItemTemplate>
                                        <asp:TextBox ID="txt_qty"   AutoPostBack="True" 
                                             ontextchanged="txt_qty_TextChanged" CssClass="form-control" TextMode="Number" Text='<%# Eval("qty") %>'  runat="server" Width="120px" AccessKey="+"></asp:TextBox>
                            
                         
                             </ItemTemplate>   </asp:TemplateField>

                               <asp:TemplateField>
                           <HeaderTemplate>Amount </HeaderTemplate>
                            <ItemTemplate>
                                     
                                <asp:Label ID="lbl_amount" runat="server" Text='<%# String.Format("{0:N}", Eval("Amount")) %>'></asp:Label>
                               <asp:HiddenField ID="hd_amount" Value='<%# Eval("Amount") %>' runat="server"></asp:HiddenField>
                             </ItemTemplate>   </asp:TemplateField>

                               <%-- <asp:BoundField DataField="Amount" HeaderText="Amount"  DataFormatString="{0:N}" />--%>
                          
                               <asp:TemplateField>
            <ItemTemplate>
                 
                      <asp:LinkButton ID="btn_deleteitemcart" CssClass="btn btn-danger btn-sm "  CommandName="Delete" runat="server"
                      OnClientClick="return getConfirmation_verifys(this, 'Please confirm','Are you sure you want to Delete?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                      </div>

                         </div>
                                <div class="col-lg-5">
                             <div class="input-group">
          <cc1:AutoCompleteExtender ServiceMethod="Searchname"
                                                    MinimumPrefixLength="2"
                                                    CompletionInterval="100" EnableCaching="False" CompletionSetCount="5"
                                                    TargetControlID="txt_customer"
                                                    ID="AutoCompleteExtender1" runat="server" 
                                       FirstRowSelected = "false" CompletionListCssClass="completionList_default"
                                                                                      CompletionListHighlightedItemCssClass="itemHighlighted"                        
                                       CompletionListItemCssClass="listItem" ServicePath="pos1.aspx">
                                                </cc1:AutoCompleteExtender>
                                <asp:TextBox ID="txt_customer" AccessKey="3" TabIndex="3" CssClass="form-control" runat="server" 
                                                        placeholder="Walk-in-Customer"
                                                          ToolTip="Search customer fullname"></asp:TextBox>
                                <div class="input-group-append">
                                           
                                        

                            <asp:LinkButton ID="btn_addc" AccessKey="A" CssClass="btn btn-primary" ToolTip="Add customer (shortcut: alt+A) "
                            runat="server"   OnClientClick="return getConfirmation_addcustomer(this, 'Add Customer');"
                                        onclick="btn_addc_Click" ValidationGroup="addcustomer"><i class="fa fa-user"></i> Add Customer </asp:LinkButton>
                                        <asp:LinkButton ID="btn_customerrecord" AccessKey="R" CssClass="btn btn-default" ToolTip="View SOA (shortcut: alt+R) " runat="server" 
                                        onclick="btn_customerrecord_Click" ><i class="fa fa-info"></i> </asp:LinkButton>
                                    
                                   
                                </div>
                              </div>
                              
                              <div class="table-responsive ">
                        <table class="table table-sm">
                        
                          <tbody>
                            <tr>
                            
                              <td>Sub Total</td>
                            
                              <td  align="right">
                                  <asp:Label ID="lbl_subtot" runat="server" Text="0.00" Font-Bold="True" ForeColor="#CC0000"></asp:Label></td>
                            </tr>
                            <tr>
                            
                              <td>
                              <asp:LinkButton ID="btn_showdiscount" OnClick="btn_showdiscount_Click" 
                                  AccessKey="O" TabIndex="7"  ToolTip="Compute Discount (shortcut: alt+O) "
                                  runat="server"><i class="fa fa-calculator"></i>Discount (-)</asp:LinkButton>
                              </td>
                              <td  align="right">
                                    <asp:Label ID="lbl_discount" runat="server" Text="0.00"></asp:Label>
                                      
                                    </td>
                             
                            </tr>
                            <tr>
                         
                              <td>
                                <asp:LinkButton ID="btn_showtax" OnClick="btn_showtax_Click" 
                                  AccessKey="U" TabIndex="7"  ToolTip="Compute Tax (shortcut: alt+U) "
                                  runat="server"><i class="fa fa-calculator"></i> Tax (+)</asp:LinkButton>
                              </td>
                              <td  align="right"><asp:Label ID="lbl_tax" runat="server" Text="0.00"></asp:Label></td>
                            </tr>
                             <tr>
                         
                              <td>Total</td>
                              <td  align="right"><asp:Label ID="lbl_total" runat="server" Text="0.00"></asp:Label></td>
                            </tr>
                             <tr>
                         
                              <td>Cash Round </td>
                              <td  align="right"><asp:Label ID="lbl_cashround" runat="server" Text="0.00"></asp:Label></td>
                            </tr>
                             <tr style="font-size: x-large; font-weight: bold">
                         
                              <td > Total Receivable</td>
                              <td  align="right"><asp:Label ID="lbl_totreceive" runat="server" Text="0.00" ForeColor="Red"></asp:Label></td>
                            </tr>
                               <tr>
                 
                              <td  align="right" colspan="2">
                           
                                <asp:TextBox ID="txt_amounttendered" CssClass="form-control-md " 
                                      PlaceHolder="Amount Tendered"  runat="server" AutoPostBack="True" 
                                      ontextchanged="txt_amounttendered_TextChanged" AccessKey="4" TabIndex="4" Width="100%" Font-Size="X-Large" ForeColor="#CC0000" Font-Bold="True"></asp:TextBox></td>
                            </tr>
                               <tr style="font-size: x-large; font-weight: bold">
                         
                              <td > Change Return                       <span><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ErrorMessage=" * Enter amount tendered!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_amounttendered" SetFocusOnError="True" ValidationGroup="addamountentered"></asp:RequiredFieldValidator></span></td>
                              <td align="right"><asp:Label ID="lbl_change" runat="server" Text="0.00" ForeColor="#FF9933"></asp:Label></td>
                            </tr>
                            <tr>
                            
                            <td colspan="2"> 
                       <div class="row">
                          <div class="col-sm-6">
                          <label> Invoice No    </label>
           
                               
                              <asp:TextBox ID="txt_invoiceno"  placeholder="Invoice No/Receipt No"  ToolTip="Charge Invoince" Font-Bold="True" runat="server" CssClass="form-control" AccessKey="5"></asp:TextBox>
                        </div>
                           <div class="col-sm-6">
                           <label> Control Number</label>
                             <div class="input-group">
                            <asp:TextBox ID="txt_salesnote"  placeholder="Control Number" Width="50%"  ToolTip="Control Number" Font-Bold="True" runat="server" CssClass="form-control" AccessKey="6"></asp:TextBox>
                                     <div class="input-group-append">
                                  <asp:LinkButton ID="btn_ctrlnos" OnClick="btn_ctrlnos_Click" 
                                  AccessKey="G" TabIndex="7"  CssClass="btn btn-default" ToolTip="Get Control Number (shortcut: alt+G) "
                                  runat="server"><i class="fa fa-search"></i></asp:LinkButton>
                                  </div>
                                  </div>
                            </div>
                        </div>
                            
                            </td>
                            </tr>
                             <tr>
                 
                              <td  align="right" colspan="2">
                                  <asp:LinkButton ID="btn_completesale" AccessKey="S" TabIndex="5"  CssClass="btn-lg-center btn-primary" ToolTip="(shortcut: alt+S) "
                            runat="server"   onclick="btn_completesale_Click" ValidationGroup="addamountentered" 
                                      Width="100%"><i class="fa fa-check"></i> Complete Sale </asp:LinkButton>
                                    
                               </td>
                            </tr>
                          </tbody>
                        </table>



                      </div>
                      </div>
                       <div class="modal fade" id="myModal_receipt" tabindex="-1" role="dialog" aria-hidden="true">
                 <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                          
                                            <h4 class="modal-title">
                                                <span id="Span1">
                                                Receipt
                                                </span>
                                            </h4>
                                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                          	<div class="row">
							<div class="col-xs-12">
								  <iframe id="frameProfile" runat="server" src="" class="framereceipt" ></iframe>
							</div><!-- /.col -->
						</div>

                                       
                                        </div>
                                        <div class="modal-footer">
                                          
                                         <asp:LinkButton ID="btn_finish" AccessKey="N" TabIndex="11"  CssClass="btn btn-primary"
                            runat="server"   onclick="btn_finish_Click"
                                   ><i class="fa fa-plus"></i> New Sale </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                </div>

                
            

                 
                     </ContentTemplate>
                     <Triggers>

                <asp:AsyncPostBackTrigger ControlID ="txt_amounttendered"  />
                </Triggers>
                     </asp:UpdatePanel>






                    </div>



                  </div>
                </div>
              </div>




                <div class="messagealert" id="alert_container"></div>









                <div id="modalpopup_customer" class="modal fade" role="dialog">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                          
                            <h4 class="modal-title">
                                <span id="spnTitle7">
                                </span>
                            </h4>
                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                        </div>
                        <div class="modal-body">
                         
                         <div class ="col-sm-12">
                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Customer Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_addcustomer" SetFocusOnError="True" ValidationGroup="addcustomer"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addcustomer"  placeholder="Fullname"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                           <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Proof of identity
                          </label>
                           <div class="col-sm-8 ">

                              <asp:DropDownList ID="txt_identification" TabIndex="3" CssClass="form-control" runat="server">
                              </asp:DropDownList>
                          
                            </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Ref/ID No
                          </label>
                           <div class="col-sm-8 ">
                               <asp:TextBox ID="txt_refno"  placeholder="Reference No" runat="server" TabIndex="1" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Affiliation
                          </label>
                           <div class="col-sm-8 ">

                              <asp:DropDownList ID="txt_affiliation" TabIndex="4" CssClass="form-control" runat="server">
                              </asp:DropDownList>
                          
                            </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Address
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addaddress"  placeholder="Address" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Contact No.
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_contact"  placeholder="Contact number" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Email
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_email"  placeholder="Email address" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Notes
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_note"  placeholder="Notes" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         </div>
                          
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                            <button type="button" id="btnConfirm7" class="btn btn-primary">
                              Submit</button>
                        </div>
                    </div>
                </div>
            </div>

             

              <div id="modalPopUp_Delete2" class="modal fade" role="dialog">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                          
                            <h4 class="modal-title">
                                <span id="spnTitle33">
                                </span>
                            </h4>
                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                        </div>
                        <div class="modal-body">
                         
                            <p>
                                <span id="spnMsg33">
                                </span>                                
                            </p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                            <button type="button" id="btnConfirm33" class="btn btn-primary">
                               Yes</button>
                        </div>
                    </div>
                </div>
            </div>

                   <div class="modal fade" id="myModal_customersoa"  role="dialog" >
                            <div class="modal-dialog">
                                    <div class="modal-content">
                                     <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                        <div class="modal-header">
                                            <h4 class="modal-title">
                                                <span id="Span3">
                                                   SOA of <asp:Label ID="lbl_customername" runat="server" Text=""></asp:Label>
                                                </span>
                                            </h4>
                                              <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                                         <div class="table-responsive">
                                              <asp:GridView ID="gv_soarecord" runat="server"  CssClass="table table-sm table-bordered table-hover" 
                                                      AutoGenerateColumns="false" AllowPaging="true" OnPageIndexChanging="gv_soarecord_OnPaging" PageSize="10" 
                                                      GridLines="None" PagerSettings-Mode="NumericFirstLast">
                                                                    <Columns>
                                                                    <asp:TemplateField>
                                                                           <HeaderTemplate> #  </HeaderTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lbl_no" runat="server" Text="<%# Container.DataItemIndex + 1 %>  "></asp:Label>
                                                                             </ItemTemplate>
                                                                           </asp:TemplateField>
                                                                            <asp:BoundField DataField="invdateonly" HeaderText="Date" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:MMM. dd, yyyy}" />
                                                                        <asp:BoundField DataField="invoiceno" HeaderText="Charge Invoice" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="ctrlno" HeaderText="Control No" ItemStyle-HorizontalAlign="Center" />
                                                                        <asp:BoundField DataField="invoicetotal" HeaderText="Amount" DataFormatString="{0:N2}"   ItemStyle-HorizontalAlign="Right" />
                                                                       
                                                                    </Columns>
                                                                </asp:GridView>
                                                            <div align="right">   <asp:Label ID="lbl_soatotal" runat="server" Text="" Font-Bold="True"></asp:Label></div>
                                              </div>
                                          </div>
                                        <div class="modal-footer">
                                          <asp:Label ID="lbl_soafooter" runat="server" Text="" CssClass="form-control-label"></asp:Label>

                                        </div>
                                     </ContentTemplate>
                                     </asp:UpdatePanel>
                                </div>
                           </div>
                         </div>


              <div class="modal fade" id="mymodal_discount"  role="dialog" >
                            <div class="modal-dialog">
                                    <div class="modal-content">
                                     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                        <div class="modal-header">
                                            <h4 class="modal-title">
                                                <span id="Span2">
                                                 Manage Discount
                                                </span>
                                            </h4>
                                              <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                                          <div class ="col-sm-12">
                                              <div class="form-group row">
                                              <label class="col-sm-4 form-control-label">Discount Type
                                                     <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                    ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                                    ControlToValidate="txt_dtype" SetFocusOnError="True" ValidationGroup="cdiscount"></asp:RequiredFieldValidator> </span>
                                              </label>
                                               <div class="col-sm-8 ">
                                              <asp:RadioButtonList ID="txt_dtype" runat="server" CellPadding="2"  CssClass="rbl"
                                                      RepeatDirection="Horizontal">
                                                      <asp:ListItem Value="P">% Percentage</asp:ListItem>
                                                      <asp:ListItem Value="A">Amount Value</asp:ListItem>
                                                  </asp:RadioButtonList>
                                                </div>
                                               </div>
                                               <div class="form-group row">
                                              <label class="col-sm-4 form-control-label">Value
                                                     <span><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                                    ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                                    ControlToValidate="txt_dvalue" SetFocusOnError="True" ValidationGroup="cdiscount"></asp:RequiredFieldValidator> </span>
                                              </label>
                                               <div class="col-sm-8 ">
                                             <asp:TextBox ID="txt_dvalue"  placeholder="Value/Rate" TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                               </div>
                                            </div>
                                          </div>
                                        <div class="modal-footer">
                                         <asp:LinkButton ID="btn_computediscount" CssClass="btn btn-primary" ValidationGroup="cdiscount" OnClick="btn_computediscount_Click" 
                                  AccessKey="T" TabIndex="7"  ToolTip="Calculate Discount (shortcut: alt+T) "
                                  runat="server"><i class="fa fa-calculator"></i> Calculate</asp:LinkButton>
                                        </div>
                                     </ContentTemplate>
                                     </asp:UpdatePanel>
                                </div>
                           </div>
                         </div>

                         <div class="modal fade" id="mymodal_tax"  role="dialog" >
                            <div class="modal-dialog">
                                    <div class="modal-content">
                                     <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                        <div class="modal-header">
                                            <h4 class="modal-title">
                                                <span id="Span4">
                                                 Manage Tax
                                                </span>
                                            </h4>
                                              <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                                          <div class ="col-sm-12">
                                              <div class="form-group row">
                                              <label class="col-sm-4 form-control-label">Tax Type
                                                     <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                                    ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                                    ControlToValidate="txt_taxtype" SetFocusOnError="True" ValidationGroup="ctax"></asp:RequiredFieldValidator> </span>
                                              </label>
                                               <div class="col-sm-8 ">
                                              <asp:RadioButtonList ID="txt_taxtype" runat="server" CellPadding="2"  CssClass="rbl"
                                                      RepeatDirection="Horizontal">
                                                      <asp:ListItem Value="P">% Percentage</asp:ListItem>
                                                      <asp:ListItem Value="A">Amount Value</asp:ListItem>
                                                  </asp:RadioButtonList>
                                                </div>
                                               </div>
                                               <div class="form-group row">
                                              <label class="col-sm-4 form-control-label">Value
                                                     <span><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                                    ErrorMessage="*" Font-Size="Small" ForeColor="#CC3300" 
                                                    ControlToValidate="txt_taxvalue" SetFocusOnError="True" ValidationGroup="ctax"></asp:RequiredFieldValidator> </span>
                                              </label>
                                               <div class="col-sm-8 ">
                                             <asp:TextBox ID="txt_taxvalue"  placeholder="Value/Rate" TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                               </div>
                                            </div>
                                          </div>
                                        <div class="modal-footer">
                                         <asp:LinkButton ID="btn_computetax" CssClass="btn btn-primary" ValidationGroup="ctax" OnClick="btn_computetax_Click" 
                                  AccessKey="Y" TabIndex="7"  ToolTip="Calculate Tax (shortcut: alt+Y) "
                                  runat="server"><i class="fa fa-calculator"></i> Calculate</asp:LinkButton>
                                        </div>
                                     </ContentTemplate>
                                     </asp:UpdatePanel>
                                </div>
                           </div>
                         </div>
                </div>
                 

      </div>
</asp:Content>

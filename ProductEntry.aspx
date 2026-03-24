<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ProductEntry.aspx.cs" Inherits="ProductEntry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
          function getConfirmation_addcategory(sender, title, message) {
              $("#spnTitle4").text(title);
            
              $('#modalPopUp_addcategory').modal('show');
              $('#btnConfirm4').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
              return false;
          }
          function getConfirmation_addunit(sender, title, message) {
              $("#spnTitle5").text(title);

              $('#modalpopup_unit').modal('show');
              $('#btnConfirm5').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
              return false;
          }
          function getConfirmation_addtax(sender, title, message) {
              $("#spnTitle6").text(title);

              $('#modalpopup_tax').modal('show');
              $('#btnConfirm6').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
              return false;
          }
          function getConfirmation_adddiscount(sender, title, message) {
              $("#spnTitle7").text(title);

              $('#modalpopup_discount').modal('show');
              $('#btnConfirm7').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
              return false;
          }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Items </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Product & Services / Items  </li>
             
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
                      <h3 class="h4">Items</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                             <div class="row">
                              <div class="col-lg-6">

                                <asp:HiddenField ID="hd_id" runat="server" />
                                     <asp:HiddenField ID="hd_baseunit" runat="server" />
                     <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Item Type
            
         
                          </label>
                         <div class="col-sm-9 form-check">
                              <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" 
                                  onselectedindexchanged="RadioButtonList1_SelectedIndexChanged" 
                                  RepeatDirection="Horizontal" Width="50%">
                                  <asp:ListItem Selected="True">Product</asp:ListItem>
                                  <asp:ListItem>Service</asp:ListItem>
                              </asp:RadioButtonList>
                                
                               </div>
                        </div>
                     
                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Item Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_itemname" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                 
                                </span>
                        
                          </label>
                          <div class="col-sm-9 ">
                            <asp:TextBox ID="txt_itemname" placeholder="Item Name"    runat="server" CssClass="form-control"></asp:TextBox>
                          </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Base Unit
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_unit" SetFocusOnError="True" ValidationGroup="add" InitialValue="Select..."></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-9 ">

                            <div class="input-group">
                             <asp:DropDownList ID="txt_unit" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                      <asp:LinkButton ID="btn_addunit" ValidationGroup="addunit" CssClass="btn btn-primary bt-lg " onclick="btn_addunit_Click" runat="server"
                      OnClientClick="return getConfirmation_addunit(this, 'Add Unit');"
                       ><i class="fa fa-plus"></i></asp:LinkButton>
                       </div>
                                    </div>



                         
                                     <small class="help-block-none">Select the smallest unit used to track this item; e.g : Each</small>
                                 </div>
           
                        </div>
                           <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Item Category
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_category" SetFocusOnError="True" ValidationGroup="add" InitialValue="Select..."></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-9 ">
                           <div class="input-group">
                             <asp:DropDownList ID="txt_category" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                      <asp:LinkButton ID="btn_addcategory" ValidationGroup="addcateg" CssClass="btn btn-primary bt-lg " onclick="btn_addcategory_Click" runat="server"
                      OnClientClick="return getConfirmation_addcategory(this, 'Add Category');"
                       ><i class="fa fa-plus"></i></asp:LinkButton>
                       </div>
                                    </div>
                             </div>
                        </div>
                     
                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Brand </label>
                           <div class="col-sm-9 ">
                          <asp:DropDownList ID="txt_brand" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                            </div>
                        </div>
                                <asp:CheckBox ID="chk_has" runat="server" Text="Has Multiple Units?" 
                                      oncheckedchanged="chk_has_CheckedChanged" AutoPostBack="True" />
                        
                   </div>
                  
                  
                         <div class="col-lg-6">
                          <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Selling Price
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_price" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-9 ">
                            <asp:TextBox ID="txt_price"  placeholder="Selling price"  TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                     <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Tax Rate
                          <span><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_taxrate" SetFocusOnError="True" ValidationGroup="add" InitialValue="Not applicable"></asp:RequiredFieldValidator>
                          
                                </span>
                        
                          </label>
                           <div class="col-sm-9 ">


                              <div class="input-group">
                         <asp:DropDownList ID="txt_taxrate" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                      <asp:LinkButton ID="btn_addtax" ValidationGroup="addtax" CssClass="btn btn-primary bt-lg " onclick="btn_addtax_Click" runat="server"
                      OnClientClick="return getConfirmation_addtax(this, 'Add Tax');"
                       ><i class="fa fa-plus"></i></asp:LinkButton>
                       </div>
                                    </div>



                            
                             </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Discount
                              <span><asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_discount" SetFocusOnError="True" ValidationGroup="add" InitialValue="Not applicable"></asp:RequiredFieldValidator>

                                </span>
                        
                          </label>
                           <div class="col-sm-9 ">
                            <div class="input-group">
                       <asp:DropDownList ID="txt_discount" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                    <div class="input-group-append">
                                      <asp:LinkButton ID="btn_adddiscount" ValidationGroup="adddiscount" CssClass="btn btn-primary bt-lg " onclick="btn_adddiscount_Click" runat="server"
                      OnClientClick="return getConfirmation_adddiscount(this, 'Add Discount');"
                       ><i class="fa fa-plus"></i></asp:LinkButton>
                       </div>
                                    </div>



                         
                                    </div>
           
                        </div>
                             <asp:Panel ID="reorder" CssClass="form-group row" runat="server">

                   
                          <label class="col-sm-3 form-control-label">Reorder Level
                             <%-- <span><asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_reorderlevel" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>

                                </span>--%>
                        
                          </label>
                           <div class="col-sm-9 ">
                            <asp:TextBox ID="txt_reorderlevel" placeholder="Re order level" TextMode="Number"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                                                   </asp:Panel>
                        <asp:Panel ID="Panel1" CssClass="form-group row" runat="server">

                   
                          <label class="col-sm-3 form-control-label">Barcode
                       
                        
                          </label>
                           <div class="col-sm-9 ">
                            <asp:TextBox ID="txt_barcode" placeholder="Barcode" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                                                   </asp:Panel>


                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Status
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_status" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                                   <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ErrorMessage="* special char. not allowed" 
                              ControlToValidate="txt_status" Font-Size="Small" 
                              ForeColor="#CC3300"  ValidationExpression="^[a-zA-Z0-9\ \&]+$"  ValidationGroup="add" SetFocusOnError="True"></asp:RegularExpressionValidator>
                                </span>
                        
                          </label>
                          <div class="col-sm-9 ">

                            <asp:DropDownList ID="txt_status" runat="server" CssClass="form-control">
                                <asp:ListItem>Active</asp:ListItem>
                                <asp:ListItem>Inactive</asp:ListItem>
                            </asp:DropDownList>
                        </div>
           
                        </div>
                         </div>
                         </div>
                                      <ajaxToolkit:CollapsiblePanelExtender runat="server" ID="cpe" 
                                      TargetControlID="Panel_content" 
                                      CollapseControlID="Panel_col" ExpandControlID="Panel_col"
                                       Collapsed="true" CollapsedSize="0" 
                                       ExpandedText="" CollapsedText="" 
                                       TextLabelID="lnkbutton" SuppressPostBack="True">
                                 </ajaxToolkit:CollapsiblePanelExtender>
    
                                <asp:Panel ID="panel_has" CssClass="row" Visible="false" runat="server" >
                                 <div class="line"></div>
                                 <div class ="col-12">

                            <h3>Related Units</h3>
                             <asp:Panel ID="Panel_col" runat="server">
                                                       <asp:LinkButton CssClass="form-control-label" ID="lnkbutton" runat="server" >Help me understand this</asp:LinkButton>
                                                     </asp:Panel>


                                    <asp:Panel ID="Panel_content" CssClass="form-control-label" runat="server">
                                    ** This option is helpful when you buy in one unit of measure, but sell in a different unit of measure. A unit of measure set consists of a base unit (usually the smallest unit used to track a certain type of item) and maximum of two related units (defined as containing a certain number of base units). For example, you could create a unit of measure set called "Pcs" and related units of Case (containing 12 Pcs) and Box (containing 36 Pcs).
                                    </asp:Panel>
                                    </div>
                                      <div class="line"></div>


                        <div class="col-lg-6">
                         <div class="form-group row">
   
                          <label class="col-sm-3 form-control-label">UOM 1
                                <span><asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_uom1" SetFocusOnError="True" ValidationGroup="add" InitialValue="Select..."></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                            
                              <asp:DropDownList ID="txt_uom1" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                     
                          </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Number of Base Units
                                <span><asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_uomno1" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                             <asp:TextBox ID="txt_uomno1" placeholder=""    runat="server" CssClass="form-control"></asp:TextBox>
           
                            
                            <small class="help-block-none">Enter the number of base unit that is equivalent to this unit</small>
                          </div>
                        </div>

                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Selling Price
                          <span><asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_selling1" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                             <asp:TextBox ID="txt_selling1" placeholder=""  TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
     
                          </div>
                        </div>
                       
                        </div>


                        <div class="col-lg-6">
                         <div class="form-group row">
   
                          <label class="col-sm-3 form-control-label">UOM 2
                                <span><asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_uom2" Enabled="false" SetFocusOnError="True" ValidationGroup="add" InitialValue="Select..."></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                            
                              <asp:DropDownList ID="txt_uom2" runat="server" CssClass="form-control" 
                                  onselectedindexchanged="txt_uom2_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                     
                          </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Number of Base Units
                                <span><asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_uomno2" Enabled="false" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                             <asp:TextBox ID="txt_uomno2" placeholder=""    runat="server" CssClass="form-control"></asp:TextBox>
           
                            
                            <small class="help-block-none">Enter the number of base unit that is equivalent to this unit</small>
                          </div>
                        </div>

                        <div class="form-group row">
                          <label class="col-sm-3 form-control-label">Selling Price
                          <span><asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" 
                                ErrorMessage=" * " Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_selling2" Enabled="false" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                          </span>
                          </label>
                          <div class="col-sm-9">
                             <asp:TextBox ID="txt_selling2" placeholder=""   TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
     
                          </div>
                        </div>
                       
                        </div>


                
                
                               

                
                
                     </asp:Panel>
                    <div class="row pull-right">
                    <div class="col-12">
                        <div class="form-group">
                                   <asp:LinkButton ID="btn_cancel" CssClass="btn btn-default" runat="server" 
                                onclick="btn_cancel_Click" ><i class="fa fa-refresh"></i> Cancel</asp:LinkButton>

                            <asp:LinkButton ID="btn_save" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_save_Click" ValidationGroup="add"><i class="fa fa-send"></i> Submit</asp:LinkButton>
                                 
                  
                        </div>
                        </div>
                      </div>

                
                     </ContentTemplate>
                     </asp:UpdatePanel>

                   

                    </div>
                  </div>
                </div>
            


            








                <div class="messagealert" id="alert_container"></div>

                 <div id="modalPopUp_addcategory" class="modal fade" role="dialog">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                          
                            <h4 class="modal-title">
                                <span id="spnTitle4">
                                </span>
                            </h4>
                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                        </div>
                        <div class="modal-body">
                         
                         <div class ="col-sm-12">
                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Category Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_addcategname" SetFocusOnError="True" ValidationGroup="addcateg"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addcategname"  placeholder="Category name"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         </div>
                          
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                            <button type="button" id="btnConfirm4" class="btn btn-primary">
                              Submit</button>
                        </div>
                    </div>
                </div>
            </div>


            <div id="modalpopup_unit" class="modal fade" role="dialog">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                          
                            <h4 class="modal-title">
                                <span id="spnTitle5">
                                </span>
                            </h4>
                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                        </div>
                        <div class="modal-body">
                         
                         <div class ="col-sm-12">
                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Unit Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_addunitname" SetFocusOnError="True" ValidationGroup="addunit"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addunitname"  placeholder="Unit name"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         </div>
                          
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                            <button type="button" id="btnConfirm5" class="btn btn-primary">
                              Submit</button>
                        </div>
                    </div>
                </div>
            </div>


            <div id="modalpopup_tax" class="modal fade" role="dialog">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                          
                            <h4 class="modal-title">
                                <span id="spnTitle6">
                                </span>
                            </h4>
                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                        </div>
                        <div class="modal-body">
                         
                         <div class ="col-sm-12">
                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Tax Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_addtax" SetFocusOnError="True" ValidationGroup="addtax"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addtax"  placeholder="Tax name"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Tax Rate
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_addtaxrate" SetFocusOnError="True" ValidationGroup="addtax"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_addtaxrate"  placeholder="Tax rate"  TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         </div>
                          
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                            <button type="button" id="btnConfirm6" class="btn btn-primary">
                              Submit</button>
                        </div>
                    </div>
                </div>
            </div>

            <div id="modalpopup_discount" class="modal fade" role="dialog">
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
                          <label class="col-sm-4 form-control-label">Discount Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_adddiscount" SetFocusOnError="True" ValidationGroup="adddiscount"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_adddiscount"  placeholder="Discount name"   runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Discount Rate
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_adddiscountrate" SetFocusOnError="True" ValidationGroup="adddiscount"></asp:RequiredFieldValidator>
                         
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_adddiscountrate"  placeholder="Discount rate"  TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
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

                </div>
                </div>
                 

      </div>
</asp:Content>





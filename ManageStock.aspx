<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ManageStock.aspx.cs" Inherits="ManageStock" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">


  <link rel="stylesheet" href="distribution/plugins/select2/css/select2.min.css">
   <link rel="stylesheet" href="distribution/plugins/select2-bootstrap4-theme/select2-bootstrap4.min.css"> 
<script type="text/javascript" src="distribution/plugins/select2/js/select2.full.min.js"></script>
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
   margin-left: 5px;
   margin-right: 5px;
}
.rbl label
{
  margin-right: 15px;
  font-weight:bold;
}
.select2-container{
        width: 100% !important;
    }
    .control-label {
        width: 100%;
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
                $("#alert_div").fadeTo(4000, 500).slideUp(500, function () {
                    $("#alert_div").remove();
                });
            }, 1000); //5000=5 seconds
        }
    </script>
        <script type="text/javascript">
            function openModalsearch() {
                $('#myModal_search').modal('show');
            }
            function openModaladdstock() {
                $('#mymodal_addstock').modal('show');
            }
            </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">


      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Manage Stock </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Inventory / Manage Stock  </li>
             
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
                     <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
                            <ContentTemplate>
                               <asp:LinkButton ID="btn_addshow"   ToolTip="Shortcut key: alt+A" AccessKey="A" CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_addshow_Click"><i class="fa fa-plus"></i> </asp:LinkButton>
                        <asp:LinkButton ID="btn_search"   ToolTip="Shortcut key: alt+S" AccessKey="S" CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_search_Click"><i class="fa fa-search"></i> </asp:LinkButton>
                                         <asp:LinkButton ID="btn_reset"  ToolTip="Shortcut key: alt+R" AccessKey="R" CssClass="btn btn-secondary" runat="server" 
                                        onclick="btn_reset_Click"><i class="fa fa-refresh"></i> </asp:LinkButton>
                                        </ContentTemplate>

                                        </asp:UpdatePanel>
                    </div>
                    <div class="card-header d-flex align-items-center">
                      <h3 class="h4"> Manage Stock</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                                <asp:HiddenField ID="hd_id" runat="server" />
                   
                         
                 <div class="table-responsive  table-sm table-hover">




                      <asp:GridView ID="gv_masterlist" runat="server" 
                                                  CssClass="table table-hover" 
                                                  AutoGenerateColumns="False" AllowPaging="True"
                                            OnPageIndexChanging="OnPaging" 
                              onrowdatabound="gv_masterlist_RowDataBound" GridLines="None" 
                          PagerSettings-Mode="NumericFirstLast">
                                            <Columns>
                                             <asp:TemplateField>
            <ItemTemplate>
              <asp:HiddenField ID="hd_salesid" Value='<%#Eval("invent_soldsalesid") %>' runat="server"></asp:HiddenField>
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("inventoryid") %>' runat="server"></asp:HiddenField>
                     <asp:HiddenField ID="hd_prodname" Value='<%#Eval("productname") %>' runat="server"></asp:HiddenField>
                 <asp:HiddenField ID="hd_prodid" Value='<%#Eval("inproductid") %>' runat="server"></asp:HiddenField>
                  <asp:HiddenField ID="hd_qty" Value='<%#Eval("inventoryqty") %>' runat="server"></asp:HiddenField>
                   <asp:HiddenField ID="hd_costunit" Value='<%#Eval("inventorycostperunit") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_note" Value='<%#Eval("inventorynote") %>' runat="server"></asp:HiddenField>
              <asp:HiddenField ID="hd_expirydate" Value='<%#Eval("invent_expirydate") %>' runat="server"></asp:HiddenField>
               <asp:HiddenField ID="hd_lotno" Value='<%#Eval("invent_lotno") %>' runat="server"></asp:HiddenField>
                <asp:HiddenField ID="hd_inventorytype" Value='<%#Eval("inventorytype") %>' runat="server"></asp:HiddenField>
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
        </asp:TemplateField><asp:BoundField DataField="inventorydate" HeaderText="Entry Date" ItemStyle-HorizontalAlign="Left" 
                                                    DataFormatString="{0:MMM. dd, yyyy}" >
                                               
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                               
                                                <asp:BoundField DataField="itemnamedesc" HeaderText="Item Description" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                 
                                                <asp:BoundField DataField="pcategname" HeaderText="Category"    
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="inventorytype" HeaderText="Type" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="inventoryqty" HeaderText="Qty" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                   <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                   <asp:BoundField DataField="inventorycostperunit" HeaderText="Cost" 
                                                    ItemStyle-HorizontalAlign="Left" DataFormatString="{0:N}" >
                                                     <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                     <asp:BoundField DataField="inventorynote" HeaderText="Notes" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="dtexpiredesc" HeaderText="Expiry" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                  <asp:BoundField DataField="invent_lotno" HeaderText="Lot No" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                 <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="ufullname" HeaderText="Entry by" 
                                                    ItemStyle-HorizontalAlign="Left" >
                                                       
                                                <ItemStyle HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                       
                                            </Columns>
                                            <PagerSettings Mode="NumericFirstLast" />
                                        </asp:GridView>

                                         <asp:Label ID="lbl_item" runat="server" Text="" CssClass="form-control-label"></asp:Label>





                        
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
                                                <span id="Span2">
                                               Search
                                                </span>
                                            </h4>
                                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                            
                         <div class ="col-sm-12">

                         
                        <div class="form-group row">
                         
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                                         <span class="input-group-text">Date Filter 
                                          
                                         </span> 

                            <asp:DropDownList ID="dp_filterby"  CssClass="form-control" runat="server" 
                                    AutoPostBack="True" onselectedindexchanged="dp_daterange_SelectedIndexChanged">

                            </asp:DropDownList>
                       
                            </div>
                            </div>
                        </div>
                      

                          <div class="form-group row">
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                       
                              <asp:TextBox ID="txt_datefrom"  placeholder="Date From" TextMode="Date"  runat="server" CssClass="form-control" ToolTip="Filter by date from" Enabled="False"></asp:TextBox>
                           
                            <div class="input-group-append">
                                <span class="input-group-text">To </span> 
                             <asp:TextBox ID="txt_dateto"  placeholder="Date To" TextMode="Date"  runat="server" CssClass="form-control"  ToolTip="Filter by date to" Enabled="False"></asp:TextBox>
                            </div>
                              </div>
                            </div>
                         </div>
                         

                        <div class="form-group row">
                         
                           <div class="col-sm-12 ">
                            <div class="input-group"> 
                     
                            <asp:TextBox ID="txt_searchkeyword"  placeholder="Enter Keywords" runat="server" CssClass="form-control"></asp:TextBox>
                             <div class="input-group-append">
                                                               <asp:DropDownList ID="txt_searchcategory" CssClass="form-control"  runat="server" ToolTip="Product Category">
                                                               </asp:DropDownList>

                                                               </div>
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
                                          
                                       
                                         <asp:LinkButton ID="btn_adsearch" AccessKey="V"  CssClass="btn btn-primary"
                            runat="server"   onclick="btn_adsearch_Click"  ValidationGroup="search"
                                   ToolTip="Shortcut key: alt+V"><i class="fa fa-search"></i> Find </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                </div>
            


            <div class="modal fade" id="mymodal_addstock"  role="dialog" >
                 <div class="modal-dialog modal-lg">
                                    <div class="modal-content">
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                        <div class="modal-header">
                          
                                            <h4 class="modal-title">
                                                <span id="Span1">
                                         <asp:Label ID="lbl_headeradd" runat="server" Text="Add Stock"></asp:Label>
                                                </span>
                                            </h4>
                                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                            
                             <div class ="col-sm-12">
                             <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Inventory Type
                                 <span><asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" ControlToValidate="txt_inventorytype"  ValidationGroup="addstock" 
                                SetFocusOnError="True" ></asp:RequiredFieldValidator>
                                </span>
                          </label>
                           <div class="col-sm-8 ">
                                  <asp:RadioButtonList ID="txt_inventorytype" runat="server" CellPadding="5"  CssClass="rbl"
                                                      RepeatDirection="Horizontal">
                                                      <asp:ListItem Value="In">In (+ Increase)</asp:ListItem>
                                                      <asp:ListItem Value="Out">Out (- Decrease)</asp:ListItem>
                                                  </asp:RadioButtonList>
                            </div>
                        </div>

                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Product Item
                                 <span><asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" ControlToValidate="txt_itemname" InitialValue="Select..."  ValidationGroup="addstock" 
                                SetFocusOnError="True" ></asp:RequiredFieldValidator>
                                </span>
                          </label>
                           <div class="col-sm-8 ">
                            <asp:DropDownList ID="txt_itemname" runat="server"  CssClass="form-control select2bs4"> </asp:DropDownList>
                            </div>
                        </div>
                           <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Quantity
                                 <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" ControlToValidate="txt_qty"  ValidationGroup="addstock" 
                                SetFocusOnError="True" ></asp:RequiredFieldValidator>
                                </span>
                          </label>
                           <div class="col-sm-8 ">
                             <asp:TextBox ID="txt_qty" placeholder="Quantity" TextMode="Number" runat="server" ToolTip="Quantity" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                           <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Cost per unit
                                 <span><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" ControlToValidate="txt_cost"  ValidationGroup="addstock" 
                                SetFocusOnError="True" ></asp:RequiredFieldValidator>
                                </span>
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_cost" placeholder="Cost per unit" TextMode="Number" runat="server" CssClass="form-control" ToolTip="Cost per unit"></asp:TextBox>
                            </div>
                        </div>
                        
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Lot Number</label>
                           <div class="col-sm-8 ">
                           <asp:TextBox ID="txt_lotnumber" placeholder="Lot Number" runat="server" ToolTip="Lot Number" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Expiry Date</label>
                           <div class="col-sm-8 ">
                           <asp:TextBox ID="txt_expirydate" TextMode="Date" runat="server" ToolTip="Ignore if not required Expiry Date" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Notes</label>
                           <div class="col-sm-8 ">
                           <asp:TextBox ID="txt_note" placeholder="Notes" runat="server" ToolTip="Notes" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                          </div>
                              </ContentTemplate>
                          <Triggers>

                <asp:AsyncPostBackTrigger ControlID ="btn_add"  />
                </Triggers>
            </asp:UpdatePanel>


                        </div>
                                        <div class="modal-footer">
                                                    
                            <asp:LinkButton ID="btn_add" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_add_Click" ValidationGroup="addstock"  ToolTip="Shortcut key: alt+A" AccessKey="A"><i class="fa fa-save"></i> Submit</asp:LinkButton>
                                  
                                        </div>
                                    </div>
                                </div>
                </div>





                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 

      </div>
       <script type="text/javascript">
           window.onload = function () {
               load_select();
           };
           //On UpdatePanel Refresh
           var prm = Sys.WebForms.PageRequestManager.getInstance();
           if (prm != null) {
               prm.add_endRequest(function (sender, e) {
                   if (sender._postBackSettings.panelsToUpdate != null) {
                       load_select();
                   }
               });
           };
           function load_select() {
               $(function () {
                   //Initialize Select2 Elements
                   $('.select2bs4').select2({
                       theme: 'bootstrap4'
                   })
               })
           };
</script>
</asp:Content>


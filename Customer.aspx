<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Customer.aspx.cs" Inherits="Customer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

 <style type="text/css">
        .messagealert {
            width:50%;
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
            $('#alert_container').append('<div id="alert_div" style="margin: 0 0.5%; -webkit-box-shadow: 3px 4px 6px #999;" class="alert fade in ' + cssclass + '"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a> <p>' + message + '</p></div>');

            setTimeout(function () {
                $("#alert_div").fadeTo(3000, 500).slideUp(500, function () {
                    $("#alert_div").remove();
                });
            }, 1000); //5000=5 seconds
        }
    </script>
     
      <script type="text/javascript">
          function openModal() {

              $('#myModal_customer').modal('show');
          }
          function openModal_record() {

              $('#myModal_customersoa').modal('show');
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
              <h2 class="no-margin-bottom">Customers</h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Customers   </li>
        
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
                      runat="server" onclick="btn_additem_Click"  ToolTip="Shortcut key: alt+A" AccessKey="A"><i class="fa fa-plus"></i></asp:LinkButton>

                                                            
                            <asp:LinkButton ID="btn_search"   ToolTip="Shortcut key: alt+S" AccessKey="S" CssClass="btn btn-primary" runat="server" 
                                        onclick="btn_search_Click"><i class="fa fa-search"></i> </asp:LinkButton>
                                         <asp:LinkButton ID="btn_reset"  ToolTip="Shortcut key: alt+R" AccessKey="R" CssClass="btn btn-secondary" runat="server" 
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
                     <asp:HiddenField ID="hd_id" Value='<%#Eval("customerid") %>' runat="server"></asp:HiddenField>
                     <asp:HiddenField ID="hd_customername" Value='<%#Eval("cfullname") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_address" Value='<%#Eval("caddress") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_contact" Value='<%#Eval("ccontact") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_email" Value='<%#Eval("cemail") %>' runat="server"></asp:HiddenField>
                    <asp:HiddenField ID="hd_status" Value='<%#Eval("cstatus") %>' runat="server"></asp:HiddenField>
                      <asp:HiddenField ID="hd_note" Value='<%#Eval("cnote") %>' runat="server"></asp:HiddenField>
                <asp:HiddenField ID="hd_identity" Value='<%#Eval("identityname") %>' runat="server"></asp:HiddenField>
                 <asp:HiddenField ID="hd_refno" Value='<%#Eval("refno") %>' runat="server"></asp:HiddenField>
                  <asp:HiddenField ID="hd_affiliation" Value='<%#Eval("affiliation") %>' runat="server"></asp:HiddenField>
                <asp:LinkButton ID="btn_selectbranch" CssClass="btn btn-primary btn-sm " onclick="btn_selectbranch_Click" runat="server" ><i class="fa fa-edit"></i></asp:LinkButton>
                      <asp:LinkButton ID="btn_deletebranch" CssClass="btn btn-danger btn-sm " onclick="btn_deletebranch_Click" runat="server"
                      OnClientClick="return getConfirmation_verify(this, 'Please confirm','Are you sure you want to Delete?');"
                       ><i class="fa fa-trash"></i></asp:LinkButton>

                           <asp:LinkButton ID="btn_crecord" CssClass="btn btn-default btn-sm " onclick="btn_crecord_Click" runat="server" ><i class="fa fa-book"></i></asp:LinkButton>
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
              <asp:BoundField DataField="refno" HeaderText="ID No" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="cfullname" HeaderText="Fullname" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="caddress" HeaderText="Address" ItemStyle-HorizontalAlign="Left" />
                                                
                                                <asp:BoundField DataField="identityname" HeaderText="Identity Type"    ItemStyle-HorizontalAlign="Left" />
                                                  <asp:BoundField DataField="affiliationname" HeaderText="Affiliation"    ItemStyle-HorizontalAlign="Left" />
                                                 <asp:BoundField DataField="cstatus" HeaderText="Status"  ItemStyle-HorizontalAlign="Left" />
                                             
                                                  <asp:BoundField DataField="cdatecreated" HeaderText="Date Added" ItemStyle-HorizontalAlign="Left" DataFormatString="{0:MMM. dd, yyyy}" />
                                                 <asp:BoundField DataField="usname" HeaderText="Entry by" ItemStyle-HorizontalAlign="Left" />
                                                       
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



             <div class="modal fade" id="myModal_customer"  role="dialog" >
                 <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                          
                                            <h4 class="modal-title">
                                                <span id="Span1">
                                               Customer Entry
                                                </span>
                                            </h4>
                                               <button type="button" data-dismiss="modal" aria-label="Close" class="close"><span aria-hidden="true">×</span></button>
                                        </div>
                                        <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                            
                         <div class ="col-sm-12">
                          
                          <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Customer Name
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" 
                                ErrorMessage=" *" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_customername"  ValidationGroup="addcustomer" 
                                  SetFocusOnError="True" ></asp:RequiredFieldValidator>
                                </span>
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_customername"  placeholder="Fullname"   runat="server" CssClass="form-control" TabIndex="2" AccessKey="1"></asp:TextBox>
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
                            <asp:TextBox ID="txt_address"  placeholder="Address" runat="server" TabIndex="5" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Contact No.
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_contact"  placeholder="Contact number" runat="server" TabIndex="6" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Email
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_email"  placeholder="Email address" runat="server" TabIndex="7" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Notes
            
                        
                          </label>
                           <div class="col-sm-8 ">
                            <asp:TextBox ID="txt_note"  placeholder="Notes" runat="server" TabIndex="8" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Status
            
                        
                          </label>
                           <div class="col-sm-8 ">
                         
                              <asp:DropDownList ID="txt_status" CssClass="form-control" TabIndex="9" runat="server">
                                  <asp:ListItem>Active</asp:ListItem>
                                  <asp:ListItem>Inactive</asp:ListItem>
                              </asp:DropDownList>
                            </div>
                        </div>
                         </div>
                          

                              </ContentTemplate>
                          <Triggers>

                <asp:AsyncPostBackTrigger ControlID ="btn_finish"  />
                </Triggers>
            </asp:UpdatePanel>


                        </div>
                                        <div class="modal-footer">
                                          
                                       
                                         <asp:LinkButton ID="btn_finish" AccessKey="G" TabIndex="10"  CssClass="btn btn-primary"
                            runat="server"   onclick="btn_finish_Click"  ValidationGroup="addcustomer"
                                   ><i class="fa fa-plus"></i> Submit </asp:LinkButton>
                                        </div>
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

                <div class="messagealert" id="alert_container"></div>
      
    
                
                </div>
                 </div>

</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CheckCashDrawer.aspx.cs" Inherits="CheckCashDrawer" %>

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
              <h2 class="no-margin-bottom">Check Cash Drawer </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Check Cash Drawer </li>
             
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
                      <h3 class="h4">Check Cash Drawer</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                               <div class="row ">
                              <div class="col-lg-6">

                                <asp:HiddenField ID="hd_id" runat="server" />

                                <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Date</label>
                          <div class="col-sm-8">
                          <asp:TextBox ID="txt_dateasof" TextMode="Date"  runat="server" CssClass="form-control"></asp:TextBox>
                       <small class="form-text"><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_dateasof" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator></small>
                          </div>
                        </div>
                        <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Beginning Balance</label>
                          <div class="col-sm-8">
                            
                             <asp:TextBox ID="txt_beginbalance"  placeholder="Beginning Balance"  TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                            <small class="form-text"> <span><asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_beginbalance" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                         
                                </span>
                        </small>
                          </div>
                        </div>
                            <div class="form-group row">
                          <label class="col-sm-4 form-control-label">Amount in Drawer</label>
                          <div class="col-sm-8">
                         <asp:TextBox ID="txt_amountdrawer"  placeholder="Amount in drawer"  TextMode="Number"    runat="server" CssClass="form-control"></asp:TextBox>
                        
                        <small class="form-text">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_amountdrawer" SetFocusOnError="True" ValidationGroup="add"></asp:RequiredFieldValidator>
                        </small>
                          </div>
                        </div>

                       
                   </div>


                  <div class="col-lg-6">
                
                   <div class="table-responsive">
                        <table class="table">
                  <tr>
                  <td>Beginning Balance</td>
                  <td> <asp:Label ID="lbl_begbal" runat="server" Text="0.00"></asp:Label></td>
                  </tr>
                    <tr>
                  <td>Cash In</td>
                  <td> <asp:Label ID="lbl_cashin" runat="server" Text="0.00"></asp:Label></td>
                  </tr>
                   <tr>
                  <td>Amount that should be in drawer</td>
                  <td> <asp:Label ID="lbl_amountshouldbe" runat="server" Text="0.00"></asp:Label></td>
                  </tr>
                   <tr>
                  <td>Your entered amount in drawer</td>
                  <td> <asp:Label ID="lbl_yourammount" runat="server" Text="0.00"></asp:Label></td>
                  </tr>
                  <tr>
                
                  <td  colspan="2" align="center" > <asp:Label ID="lbl_remarks" runat="server" Text="" Font-Size="Large" Font-Bold="True"></asp:Label></td>
                  </tr>
                  </table>

                  </div>
                  </div>
                   
                         
              </div>
                        
                    <div class="row pull-right">
                    <div class="col-12">
                        <div class="form-group">
                               
                                 <asp:LinkButton ID="btn_cancel" CssClass="btn btn-default" runat="server" 
                                onclick="btn_cancel_Click" ValidationGroup="add2"><i class="fa fa-refresh"></i> Cancel</asp:LinkButton>
                                 
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
            


            








                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 

      </div>
</asp:Content>





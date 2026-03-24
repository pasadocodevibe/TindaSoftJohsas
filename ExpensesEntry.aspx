<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ExpensesEntry.aspx.cs" Inherits="ExpensesEntry" %>

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
<asp:Content ID="Content2" runat="server" 
    contentplaceholderid="body">
      <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Expense Entry   </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Expense Entry  </li>
             
            </ul>
         
          </div>
          <br />
          <div class="forms">
            
     
                
         <div class="container-fluid" >
              <div class="row">
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
                      <h3 class="h4">Expense Entry</h3>
                    </div>
                    <div class="card-body">

                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                             <asp:HiddenField ID="hd_id" runat="server" />
               
                     
                        <div class="form-group">
                          <label class="form-control-label">Date
             <span><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                ErrorMessage=" * required!" Font-Size="Small" ForeColor="#CC3300" 
                                ControlToValidate="txt_date" SetFocusOnError="True"></asp:RequiredFieldValidator>
                                       <asp:RangeValidator id="valDateRange" runat="server"  ControlToValidate="txt_date" 
                                           Type ="Date" Display="Dynamic" ErrorMessage="Date not allowed!" Font-Size="Small" ForeColor="orange"  />
                                
                                </span>
                        
                          </label>
                         

                  
                            <asp:TextBox ID="txt_date"  TextMode="Date"  runat="server" CssClass="form-control"></asp:TextBox>
           
                        </div>
                     <div class="form-group">       
                          <label class="form-control-label">Item Expense
                               <span><asp:RequiredFieldValidator ID="RequiredFieldValidator2" 
                              runat="server" ErrorMessage=" * required!" Font-Size="Small" 
                              ForeColor="#CC3300" ControlToValidate="txt_expense" SetFocusOnError="True" InitialValue="Select..."></asp:RequiredFieldValidator></span>
                     
                          </label>
                          
                            <asp:DropDownList ID="txt_expense" CssClass="form-control" runat="server">
                            </asp:DropDownList>
                        </div>
                         <div class="form-group">       
                          <label class="form-control-label">Amount
                              <span><asp:RequiredFieldValidator ID="RequiredFieldValidator3" 
                              runat="server" ErrorMessage=" * required!" Font-Size="Small" 
                              ForeColor="#CC3300" ControlToValidate="txt_amount" SetFocusOnError="True" ></asp:RequiredFieldValidator></span>
                        
                       </label> 

                              <asp:TextBox ID="txt_amount" placeholder="Amount" TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>
                       
                       </div>
                         <div class="form-group">       
                          <label class="form-control-label">Note 
                             
                        
                       </label> 

                              <asp:TextBox ID="txt_note" placeholder="Note" TextMode="MultiLine"  runat="server" CssClass="form-control"></asp:TextBox>
                       
                       </div>
                    
                        <div class="form-group">
                               
                            <asp:LinkButton ID="btn_submit" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_submit_Click">Submit</asp:LinkButton>
                      
                        </div>
                     </ContentTemplate>
                     </asp:UpdatePanel>
                    </div>
                  </div>
                </div>
                </div>
            
                <div class="messagealert" id="alert_container"></div>
                </div>
                </div>
                 
    </asp:Content>





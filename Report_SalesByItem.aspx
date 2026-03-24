<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Report_SalesByItem.aspx.cs" Inherits="Report_SalesByItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    $(function () {


        $('input[id*="txtDateRange"]').daterangepicker({
            autoUpdateInput: true,
            opens: 'center',
            timePicker: false,
            showDropdowns: false,
            timePicker24Hour: true,
            timePickerIncrement: 15,
            startDate: moment().subtract(29, 'days'),
            endDate: moment(),
            locale: {
                separator: "-",
                format: 'MM/DD/YYYY',
                "firstDay": 1
            },
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                'This Year': ['01/01/' + moment('year'), moment()]
            }
        });
    });
    </script>
  
    <script type="text/javascript">
        $(function () {

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(PageLoadHandler);

            function PageLoadHandler(sender, args) {
                $('input[id*="txtDateRange"]').daterangepicker({
                    autoUpdateInput: true,
                    opens: 'center',
                    timePicker: false,
                    showDropdowns: false,
                    timePicker24Hour: true,
                    timePickerIncrement: 15,
                    startDate: moment().subtract(29, 'days'),
                    endDate: moment(),
                    locale: {
                        separator: "-",
                        format: 'MM/DD/YYYY',
                        "firstDay": 1
                    },
                    ranges: {
                        'Today': [moment(), moment()],
                        'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                        'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                        'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                        'This Month': [moment().startOf('month'), moment().endOf('month')],
                        'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                        'This Year': ['01/01/' + moment('year'), moment()]
                    }
                });
            }
        });
    </script>
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
              <h2 class="no-margin-bottom">Report </h2>
           
            </div>
          </header>

             <div class="breadcrumb-holder container-fluid">
            <ul class="breadcrumb">
              <li class="breadcrumb-item"><a href="Home.aspx">Home</a></li>
              <li class="breadcrumb-item active">Sales Summary By Item </li>
             
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
                      <h3 class="h4"> Sales Summary By Item</h3>
                    </div>
                    <div class="card-body">
                        <asp:UpdatePanel ID="UpdatePanel_formbody" runat="server" >
                            <ContentTemplate>
                                <asp:HiddenField ID="hd_id" runat="server" />
                   <div class="form-group row">
                          <label class="col-sm-2 form-control-label">Filter By date
                          
                          
                          </label>
                          <div class="col-sm-10">
                            <div class="row">
                              <div class="col-sm-10">
                                        <asp:TextBox runat="server" ID="txtDateRange" CssClass="form-control" />  
                              </div>
                           
                             
                              <div class="col-md-2">
                                  <asp:LinkButton ID="btn_generate" CssClass="btn btn-primary" runat="server" 
                                onclick="btn_generate_Click"><i class="fa fa-send"></i> Generate</asp:LinkButton>
                              </div>
                              
                            </div>
                          </div>
                        </div>
                   
                         
                                   
                 	<div class="row">
							<div class="col-xs-12">
								  <iframe id="frameProfile" runat="server" src="" class="frame"></iframe>
							</div><!-- /.col -->
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



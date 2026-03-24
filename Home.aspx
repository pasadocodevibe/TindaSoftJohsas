<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript" src="https://www.google.com/jsapi"></script>
<script type="text/javascript">
    google.load("visualization", "1", { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var options = {
            title: '',
            width: 440,
            height: 180,
            bar: { groupWidth: "50%" },
            legend: { position: "none" },
            isStacked: true
        };
        $.ajax({
            type: "POST",
            url: "Home.aspx/GetChartData",
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (r) {
                var data = google.visualization.arrayToDataTable(r.d);
                var chart = new google.visualization.ColumnChart($("#chart")[0]);
                chart.draw(data, options);
            },
            failure: function (r) {
                alert(r.d);
            },
            error: function (r) {
                alert(r.d);
            }
        });
    }
</script>

 <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>  
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
<asp:Content ID="Content2" runat="server" contentplaceholderid="body">


         <header class="page-header">
            <div class="container-fluid">
              <h2 class="no-margin-bottom">Dashboard</h2>
            </div>
          </header>
          <br />


          <!-- 1st row section start-->
        <div class="projects no-padding-top">
            <div class="container-fluid" >
              <div class="row">
                <!-- Work Amount  -->
                <div class="col-lg-3">
                  <div class="work-amount card">
                    <div class="card-close">
                     
                    </div>
                    <div class="card-body">
                      <h3>Income & Expenses</h3>
                      <small>Today</small>
                      <div class="table-responsive">   
                        <table class="table table-sm"> 
                          <tbody>
                            <tr>  
                              <td>Income</td>
                              <td align="right"><asp:Label ID="lbl_ieincome" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Expense</td>
                              <td align="right"><asp:Label ID="lbl_ieexpense" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            
                          </tbody>
                        </table>
                        <small>This month</small>
                    
                        <table class="table table-sm"> 
                          <tbody>
                            <tr>  
                              <td>Income</td>
                              <td align="right"><asp:Label ID="lbl_ieincomemonth" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Expense</td>
                              <td align="right"><asp:Label ID="lbl_ieexpensemonth" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            
                          </tbody>
                        </table>
                      </div>
                    </div>
                  </div>
                </div>
                <!-- Client Profile -->
                <div class="col-lg-3">
                  <div class="client card">
                    <div class="card-close">
                      
                    </div>
                    <div class="card-body">
                      <h3>Quick Stat</h3>
                     <div class="table-responsive">   
                        <table class="table table-sm"> 
                          <tbody>
                            <tr>  
                              <td>Products</td>
                              <td align="right"><asp:Label ID="lbl_qproduct" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Services</td>
                              <td align="right"><asp:Label ID="lbl_qservice" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Customers</td>
                              <td align="right"><asp:Label ID="lbl_qcustomer" runat="server" Text="0"></asp:Label> </td>
                            </tr>
                              <tr>
                              <td>Users</td>
                              <td align="right"><asp:Label ID="lbl_qusers" runat="server" Text="0"></asp:Label> </td>
                            </tr>
                          </tbody>
                        </table>

                      </div>
                    </div>
                  </div>
                </div>
                <!-- Total Overdue             -->
                <div class="col-lg-6">
                  <div class="work-amount card">
                    <div class="card-close">
                    </div>
                    <div class="card-body">
                      <h3>Sales Today</h3>
                    <div class="table-responsive">   
                        <table class="table table-sm"> 
                          <tbody>
                            <tr>  
                              <td>Total Sales</td>
                              <td align="right"><asp:Label ID="lbl_tstotsales" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Transaction</td>
                              <td align="right"><asp:Label ID="lbl_tstransaction" runat="server" Text="0"></asp:Label></td>
                            </tr>
                            <tr>
                              <td>Items Sold</td>
                              <td align="right"><asp:Label ID="lbl_tsitemsold" runat="server" Text="0"></asp:Label> </td>
                            </tr>
                              <tr>
                              <td>Cash In</td>
                              <td align="right"><asp:Label ID="lbl_tscashin" runat="server" Text="0"></asp:Label> </td>
                            </tr>
                          </tbody>
                        </table>
                      </div>
                      </div>
                    </div>
                  </div>

               
            </div>
          </div>
               <!-- 1st row section end-->
</div>

                  <!-- 2nd row section start-->
        <div class="projects no-padding-top">
            <div class="container-fluid">
              <div class="row">
                
                
                <!-- Client Profile -->
                <div class="col-lg-6">
                  <div class="client card">
                    <div class="card-close">
                     
     
                    </div>
                    <div class="card-body">
                      <h3>Top 5 Sold Items</h3>


                     <div class="table-responsive">   


                     <asp:GridView ID="gv_top10solditems" runat="server"  CssClass="table table-sm" 
                            AutoGenerateColumns="false"  GridLines="None" ShowHeader="False" >
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                                                
                                                <asp:BoundField DataField="count" HeaderText=""  DataFormatString="{0:N}"  ItemStyle-HorizontalAlign="Right" />
                         
                                                       
                                            </Columns>
                                        </asp:GridView>
                       
                      </div>
                    </div>
                  </div>
                </div>
           <div class="col-lg-6">
                  <div class="client card">
                    <div class="card-close">
                     
          
                    </div>
                    <div class="card-body">
                      <h3>Low in Stock Items</h3>


                     <div class="table-responsive">   


                                      <asp:GridView ID="gv_lowinstock" runat="server"  CssClass="table table-sm" 
                                        AutoGenerateColumns="false"  GridLines="None" ShowHeader="False" >
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="remainingqty" HeaderText=""  DataFormatString="{0:N}"  ItemStyle-HorizontalAlign="Right" />  
                                            </Columns>
                                        </asp:GridView>
                       
                      </div>
                    </div>
                  </div>
                </div>
                </div>
              </div>
         
          </div>
               <!-- 2nd row section end-->






                   <!-- 3rd row section start-->
        <div class="projects no-padding-top">
            <div class="container-fluid">
              <div class="row">
                <div class="col-lg-6">
                  <div class="work-amount card">
                    <div class="card-close">
                    </div>
                    <div class="card-body">
                      <h3>Top 5 Expenses</h3>
                    <div class="table-responsive">   

                      <asp:GridView ID="gv_top5expenses" runat="server"  CssClass="table table-sm" 
                                        AutoGenerateColumns="false"  GridLines="None" ShowHeader="False" >
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="amount" HeaderText=""  DataFormatString="{0:N}"  ItemStyle-HorizontalAlign="Right" />  
                                            </Columns>
                                        </asp:GridView>
                        
                      </div>
                      </div>
                    </div>
                  </div>
                
                <div class="col-lg-6">
                  <div class="work-amount card">
                    <div class="card-close">
                    </div>
                    <div class="card-body">
                      <h3>Near Expiry</h3>
                    <div class="table-responsive">   

                      <asp:GridView ID="gv_expiry" runat="server"  CssClass="table table-sm" 
                                        AutoGenerateColumns="false"  GridLines="None" ShowHeader="False" >
                                            <Columns>
                                                <asp:BoundField DataField="itemnamedesc" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                                                    <asp:BoundField DataField="pcategname" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                                                        <asp:BoundField DataField="dtexpiredesc" HeaderText="" ItemStyle-HorizontalAlign="Left" />
                 
                                            </Columns>
                                        </asp:GridView>
                        
                      </div>
                      </div>
                    </div>
                  </div>
              
                </div>
              </div>
         
          </div>
               <!-- 3rd row section end-->
             








                 <div class="messagealert" id="alert_container"></div>
</asp:Content>



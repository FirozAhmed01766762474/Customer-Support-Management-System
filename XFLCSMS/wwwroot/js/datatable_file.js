
//$(document).ready(function () {
//    GetCustomer();
//});

//function GetCustomer() {
//    $.ajax({
//        url: '/DataTable/GetList',
//        type: 'GET',
//        dataType: 'json',
//        success: OnSuccess
//    });
//}

//function OnSuccess(response) {
//    $('#dataTabledata').DataTable({
//        processing: true,
//        lengthChange: true,
//        lengthMenu: [[5, 10, 25, -1], [5, 10, 25, "All"]],
//        filter: true,
//        sort: true,
//        paginate: true,
//        data: response,
//        columns: [
           
//            {
//                data: 'Cust_Id',
//                render: function (data, type, row, meta) {
//                    return row.Cust_Id;
//                }
//            },
//            {
//                data: 'FirstName',
//                render: function (data, type, row, meta) {
//                    return row.FirstName;
//                }
//            },
//            {
//                data: 'LastName',
//                render: function (data, type, row, meta) {
//                    return row.LastName;
//                }
//            },
//            {
//                data: 'Gender',
//                render: function (data, type, row, meta) {
//                    return row.Gender;
//                }
//            },
//            {
//                data: 'Age',
//                render: function (data, type, row, meta) {
//                    return row.Age;
//                }
//            },
//            {
//                data: 'Country',
//                render: function (data, type, row, meta) {
//                    return row.Country;
//                }
//            }
//        ]
//    });
//}
$(document).ready(function () {
    $('#dataTabledata').DataTable();
})




//$(document).ready(function () {
//    $.ajax({
//        url: '/DataTable/GetList',
//        type: 'GET',
//        dataType: 'json',
//        success: function (response) {
//            // Handle the response data, e.g., populate the table
//            debugger;

//             Example: Assuming your response is an array of objects with properties
//             Populate the DataTable with the response data
//            $('#dataTabledata').DataTable({
//                data: response,
//                columns: [
//                    { data: 'PropertyName1' },
//                    { data: 'PropertyName2' },
//                    // Add more columns as needed
//                ]
//            });
//        },
//        error: function (error) {
//            console.error('Error fetching data:', error);
//        }
//    });
//});

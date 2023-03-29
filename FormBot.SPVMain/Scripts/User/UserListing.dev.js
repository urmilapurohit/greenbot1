function LoadSpvUsers() {
    $("#datatableAllSpvUser").kendoGrid({
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: urlGetAllSpvUser,
                    //data: { 'ManufacturerId': $("#spvPanelManufacturer").val() },
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST',
                    async: false
                }
                ,
                parameterMap: function (options) {
                    options.Name = $("#txtName").val();
                    options.IsActive = $("#btnchkIsActive").prop('checked');
                    options.UserName = $("#txtUserName").val();
                    options.Email = $("#txtEmail").val();
                    options.SpvUserTypeId = $("#SpvUserTypeId").val();
                    options.SpvRoleId = $("#SpvRoleId").val();
                    return JSON.stringify(options);
                }
            },
            schema: {
                data: "data", // records are returned in the "data" field of the response
                total: "total"
            },
            pageSize: 10,
            serverPaging: true,
            serverFiltering: true,
            Sorting: true,
        },
        noRecords: true,
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100]
        },       
        columns: [{
            template: "#= ++rowNumber #",
            field: "",
            title: "No.",
            width: "60px"
           
        },
        
            {
                field: "SpvUserId",
                title: "SpvUserId",
                width: "100px",
                hidden:true
                
            },
        {
            field: "Name",
            title: "Name",
            width: "100px"
        },
        
        {
            field: "UserName",
            title: "UserName",
            width: "100px"
            },
            {
                field: "UserTypeName",
                title: "UserType",
                width: "100px"
            },
            {
                field: "IsActive",
                title: "IsActive",
                width: "100px",
               
                template: "#= kendo_IsActiveUser(data)#"
                //template: "#if(IsActive == null) { # # } else If(IsActive == true) { # { #'<img src="../Images/active.png">'<span>#=ChoiceCode#</span></div># }else { # <span>#=secondChoiceCode#</span> # } # } #"
            },
            {
                field: "Email",
                title: "Email",
                width: "100px"
            },
            {
                field: "strLastLogIn",
                title: "LastLogIn", 
                width: "100px"
            },
            {
                field: "Action",
                title: "Action",
                width: "100px",
                //template: '<span onclick="editUser" class="edituser"></span>'
                template: "#=kendo_ActionUserEditDelete(data)#"
                //template: "#if(IsActive == null) { # # } else If(IsActive == true) { # { #'<img src="../Images/active.png">'<span>#=ChoiceCode#</span></div># }else { # <span>#=secondChoiceCode#</span> # } # } #"
            },
        ],
        dataBinding: function () {
            rowNumber = (this.dataSource.page() - 1) * this.dataSource.pageSize();
        }
    });
}
function kendo_ActionUserEditDelete(data) {
    return '<span style="cursor:pointer"><img src="../Images/edit-icon.png" onclick="editUser(' + data.SpvUserId + ')"/></span>' + '&nbsp&nbsp' + '<span style="cursor:pointer" ><img src="../Images/delete-icon.png" onclick="deleteUser(' + data.SpvUserId +')"/></span>';
}
function editUser(SpvUserId) {
   
    return window.location.href = "/SpvUser/Create/" + SpvUserId ;//'@Url.Action("Create", "SpvUser")';
}

function deleteUser(SpvUserId) {
    var result = confirm("Are you sure you want to delete this record ?");
    if (result) {
        $.ajax({
            url: '/SpvUser/DeleteUser',
            type: "POST",
            async: false,
            data: JSON.stringify({ SpvuserId: SpvUserId }),
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.success) {
                    $(".alert").hide();
                    $("#errorMsgRegion").html(closeButton + "User deleted successfully. ");
                    $("#errorMsgRegion").show();

                    LoadSpvUsers();
                }
            },
        });
    }
}
function kendo_IsActiveUser(data) {
    if (data.IsActive == true) {
        return '<img src="../Images/active.png"/>';
    }
    else {
        return '<img src="../Images/inactive.png"/>';
    }
}
function ResetSpvUsers() {
    $("#txtName").val('');
    $("#btnchkIsActive").prop('checked','true');
    $("#txtUserName").val('');
    $("#txtEmail").val('');
    $("#SpvUserTypeId").val('');
    $("#SpvRoleId").val('');
    LoadSpvUsers();
}
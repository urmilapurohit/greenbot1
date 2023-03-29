$(document).ready(function () {
    debugger;
    //BindUserList();//for binding related to user dropdown
    bindnotestype();
    searchuserhistory();

    $('#UserHistorySearch').on('keyup', function () {
        debugger;
        searchuserfilter();
    });

    $('#filter-notestype').on("change", function () {
        searchuserhistory();
    });

    $('#IsDeletedUserNote').on("change", function () {
        searchuserhistory();
    });

    $('#refreshUserHistory').on("click", function () {
        searchuserhistory();
    });

    $("#FilterIsImportantNote").change(function () {
        debugger;
        $('#UserHistorySearch').val("");
        searchuserhistory();
    });

    $("#relatedTofilter").on("change", function () {
        debugger;
        searchuserfilter();
        $("#width_tmp_option").html($('#relatedTofilter option:selected').text());
        $(this).width($("#width_tmp_select").width());

    });

});


function bindnotestype() {
    $.ajax({
        type: 'GET',
        url: urlGetUserNotesType,
        dataType: 'json',
        async: false,
        data: {},
        success: function (notestype) {
            debugger;
            $.each(notestype, function (i, res) {
               
                    $("#filter-notestype").append('<option value="' + res.Value + '">' + res.Text + '</option>');
            });
            $("#filter-notestype").append('<option value="1">History</option>')
            $("#filter-notestype").append('<option value="5">Warning</option>');

        },
        error: function (ex) {
            alert('Failed to retrieve Notestype list.' + ex);
        }
    });
}

function NotesTypeFilter() {
    $.ajax({
        type: 'GET',
        url: urlGetUserNotesType,
        dataType: 'json',
        async: false,
        data: {},
        success: function (notestype) {
            debugger;
            $("#NotesType option").remove();
            $.each(notestype, function (i, res) {
                if (res.Value != "0") {
                    $("#NotesType").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                }
            });
            debugger;
            if (ProjectSession_UserTypeId == 1 || ProjectSession_UserTypeId == 3) {
                $("#NotesType").append('<option value="5">Warning</option>');
            }

        },
        error: function (ex) {
            alert('Failed to retrieve Notestype list.' + ex);
        }
    });
}

function searchuserhistory() {
    debugger;
    
    var HistoryCategory;
    var IsImportant;
    var IsDeletedUserNote;

    if (userTypeId != 4 && userTypeId != 7) {
        IsImportant = false;
        IsDeletedUserNote = 1;
        HistoryCategory = 1;
    }
    else {
        IsImportant = document.getElementById("FilterIsImportantNote").checked;
        IsDeletedUserNote = $("#IsDeletedUserNote").val();
        HistoryCategory = $('#filter-notestype').val();
    }
    setTimeout(function () {
        $.ajax({
            url: urlSearchUserHistory,
            type: "GET",
            data: { "UserID": urlUserID, "CategoryID": HistoryCategory, "IsImportant": IsImportant, "IsDeletedUserNote": IsDeletedUserNote },
            cache: false,
            async: false,
            success: function (Data) {
                debugger;
                $("#showfiltereduserhistory").html($.parseHTML(Data));
                searchuserfilter();
            }
        });

        $("#loading-image").hide();
    }, 10);

}

function searchuserfilter() {
    var searchboxfilter;
    var tagged;
   
    if (userTypeId != 4 && userTypeId != 7) {
        searchboxfilter = $("#UserHistorySearch").val();
        tagged = "0";
    }
    else {
        searchboxfilter = $("#UserHistorySearch").val();
        //tagged = $("#relatedTofilter").val();
        tagged = "0";
    }
    var searchfilter = "@" + tagged;
    var count = 0;
    if (tagged == "0") {
        $('#divCustom .history-box').each(function () {
            if ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0) {
                $(this).hide();
            } else {
                $(this).show();
                $("#norecords").empty();
                count++;
            }
        });
    }
    else {
        $('#divCustom .history-box').each(function () {
            if (($(this).text().search(new RegExp(searchfilter, "i")) < 0) || ($(this).text().search(new RegExp(searchboxfilter, "i")) < 0)) {
                $(this).hide();
            } else {
                $(this).show();
                $("#norecords").empty();
                count++;
            }
        });
    }
    if (count > 0) {
        $(".history-box").css(
            "border-top", "1px solid #e3e3e3");
    }
    else {
        $("#norecords").empty();
        var norecords = "<div style='text-align:center;font-size:24px;margin-top:120px'>" + "No Record(s) Found." + "</div>";
        $("#norecords").append(norecords);
    }
}

function BindUserList() {
    $.ajax({
        type: 'GET',
        url: urlGetAllUserList,
        dataType: 'json',
        async: false,
        data: { userId: urlUserID, UserTypeId: userTypeId },
        success: function (userlist) {
            debugger;

            $("#relatedTofilter").prepend('<option value="' + 0 + '"> All </option>')
            $.each(userlist, function (i, res) {
                debugger;
                if (res.Value != " ") {
                    $("#relatedTofilter").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                }

            });


        },
        error: function (ex) {
            alert('Failed to retrieve User list.' + ex);
        }
    });
}




$("#usernotecontainer").resizable({
    minHeight: 300,
    handles: 's',
    alsoResize: '#showfiltereduserhistory'
});



function EditUserNote(Notes) {
    debugger;
    var Noteid = $(Notes).attr("data-usernoteid");
    var UserID = urlUserID;
    var IsWarningNote = $(Notes).attr("data-IsWarningNote");
    $.ajax({
        type: 'GET',
        url: urlEditUserNote,
        dataType: 'json',
        async: false,
        data: { NoteID: Noteid, UserID: UserID, IsWarningNote: IsWarningNote },
        success: function (result) {
            var fooCallback = function () {
                CKEDITOR.instances.contentusernoteeditor.focus();
                CKEDITOR.instances.contentusernoteeditor.insertHtml(result.Notes);
            };
            CKEDITOR.instances.contentusernoteeditor.setData("", fooCallback);
            debugger;
            NotesTypeFilter();
            $("#NotesType").val(result.NotesType);
            if (result.NotesType == "5") {
                $("#NotesType").prop("disabled", true);
            }
            else {
                $("#NotesType").prop("disabled", false);
                $("#NotesType option:last").remove();
            }
            $("#hiddenUserNoteID").val(Noteid);
            $("#IsImportantNote").prop('checked', result.IsImportantNote)
        }
    })
}

function DeleteUserNote(Notes) {
    var Noteid = $(Notes).attr("data-usernoteid");
    var UserID = urlUserID;
    var IsWarningNote = $(Notes).attr("data-IsWarningNote");
    $.ajax({
        type: 'GET',
        url: urlDeleteUserNote,
        dataType: 'json',
        async: false,
        data: { NoteID: Noteid, UserID: UserID, IsWarningNote: IsWarningNote },
        success: function (result) {
            if (result.status) {
                showSuccessMessageHistoryUserNotes(result.message);
                searchuserhistory();
            }
            else {
                showErrorMessageHistoryUserNotes(result.message);
            }

        },
        error: function (ex) {
            showErrorMessageUserNotes("Failed to delete Job Note")
        }
    });
}


function PublishUserNote(Notes) {
    var Noteid = $(Notes).attr("data-usernoteid");
    var UserID = urlUserID;
    var IsWarningNote = $(Notes).attr("data-IsWarningNote");
    $.ajax({
        type: 'GET',
        url: urlpublishNote,
        dataType: 'json',
        async: false,
        data: { NoteID: Noteid, UserID: UserID, IsWarningNote: IsWarningNote },
        success: function (result) {
            if (result.status) {
                showSuccessMessageHistoryUserNotes(result.message);
                searchuserhistory();
            }
            else {
                showErrorMessageHistoryUserNotes(result.message);
            }

        },
        error: function (ex) {
            showErrorMessageUserNotes("Failed to publish Job Note");
        }
    });
}
function showSuccessMessageHistoryUserNotes(message) {
    $("#errorMsgRegionHistoryNotes").hide();
    $("#successMsgRegionHistoryNotes").html(closeButton + message);
    $("#successMsgRegionHistoryNotes").show();
}
function showErrorMessageHistoryUserNotes(message) {
    $("#successMsgRegionHistoryNotes").hide();
    $("#errorMsgRegionHistoryNotes").html(closeButton + message);
    $("#errorMsgRegionHistoryNotes").show();
}
    
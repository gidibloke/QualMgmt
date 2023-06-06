// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let AdminIndex = function () {
    $(document).ready(function () {

        var url = $('#AdminIndex').val();
        var table = $('#Users').DataTable({
            language: {
                "emptyTable": "No registered users"
            },
            pageLength: 10,
            pagingType: "numbers",
            ajax: {
                "url": url,
                "type": "GET",
                "dataType": "JSON"
            },
            "fnInitComplete": function (oSettings, json) {
                //console.log(json);
            },
            columns: [
                { data: 'UserId', name: 'UserId' },
                { data: 'Email', name: 'Email' },
                { data: 'Fullname', name: 'Fullname' },
                { data: 'CareHomeName', name: 'CareHomeName' },
                { data: 'RoleName', name: 'RoleName' },
                { data: 'EmailConfirmed', name: 'EmailConfirmed' },
                { data: 'IsEnabled', name: 'IsEnabled' },
            ],
            columnDefs: [
                { "className": "text-center", "targets": "_all" },
                {
                    targets: 0,
                    visible: false,
                },
                {
                    targets: 1,
                    render: function (a, b, data, d) {
                        var url = ``;
                        if (data.EmailConfirmed === true) {
                            url = `<div class=""><span class="badge rounded-pill bg-success">Confirmed</span></div>`
                        } else {
                            url = `<a href="#" data-toggle="modal" data-target="#confirmResend" data-id = "` + data.UserId + `"><span class="badge rounded-pill bg-primary">Resend confirmation</span></a>`
                        }
                        data = `<div class="d-flex flex-column">
                                      <div class="">`+ data.Email + `</div>` + url + `
                                    </div>`
                        return data;
                    }
                },
                {
                    targets: 5,
                    render: function (a, b, data, d) {

                        url = ``
                        if (data.Emailconfirmed === false) {
                            url += `<div class=""><span class="badge badge-info">Send Confirmation</span></div>`
                        }
                        if (data.IsEnabled === true) {
                            url += `<a href="#"  data-bs-toggle="modal" data-bs-target="#confirmDisable" data-id = "` + data.UserId + `"><span class="badge rounded-pill bg-danger">Disable User</span></a>`
                        }
                        else {
                            url += `<a href="#"  data-bs-toggle="modal" data-bs-target="#confirmEnable" data-id = "` + data.UserId + `"><span class="badge rounded-pill bg-success">Enable User</span></a>`
                        }
                        url += ` <a href="/UserManagement/Edit/` + data.UserId + `"><span class="badge rounded-pill bg-info">Edit User</span></a>`
                        return url;
                    }
                },
                {
                    targets: 6,
                    visible: false
                },
            ]
        })
    });

    var disableUser = $('#DisableUser').val();
    var enableUser = $('#EnableUser').val();
    var unitId = 0;
    $('#confirmDisable').on('show.bs.modal', function (e) {
        unitId = $(e.relatedTarget).data('id');
    })
    $('#confirmDisable').find('.modal-footer #confirm').on('click', function () {
        var urlToDisable = `${disableUser}/${unitId}`;
        console.log(urlToDisable);
        $.ajax(urlToDisable, {
            type: "POST",
            data: {
                id: unitId,
                __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
            }
        }).done(function (response) {
            $('#confirmDisable').modal('hide');
            if (response.success) {
                runAlertSuccess("User disabled")
                $('#Users').DataTable().ajax.reload();
            } else {
                runAlertError("Operation failed, please try again later");
            }
        }).fail(function (response) {
            $('#confirmDisable').modal('hide');
            runAlertError("Operation failed, please try again later");
            $('#Users').DataTable().ajax.reload();
        })
    });

    $('#confirmEnable').on('show.bs.modal', function (e) {
        unitId = $(e.relatedTarget).data('id');
    })



    $('#confirmEnable').find('.modal-footer #confirm').on('click', function () {
        var urlToEnable = `${enableUser}/${unitId}`;
        console.log(urlToEnable);
        $.ajax(urlToEnable, {
            type: "POST",
            data: {
                id: unitId,
                __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
            }
        }).done(function (response) {
            $('#confirmEnable').modal('hide');
            if (response.success) {
                runAlertSuccess("User enabled")
                $('#Users').DataTable().ajax.reload();
            } else {
                runAlertError("Operation failed, please try again later");
            }
        }).fail(function (response) {
            $('#confirmEnable').modal('hide');
            runAlertError("Operation failed, please try again later");
            $('#Users').DataTable().ajax.reload();
        })
    });

    $('#confirmResend').on('show.bs.modal', function (e) {
        unitId = $(e.relatedTarget).data('id');
    })

    $('#confirmResend').find('.modal-footer #confirm').on('click', function () {
        var urlToResend = `${resendEmail}/${unitId}`;
        console.log(urlToResend);
        $.ajax(urlToResend, {
            type: "POST",
            data: {
                id: unitId,
                __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
            }
        }).done(function (response) {
            $('#confirmResend').modal('hide');
            if (response.success) {
                runAlertSuccess("Email Resent Successfully")
                $('#Users').DataTable().ajax.reload();
            } else {
                runAlertError("Operation failed, please try again later");
            }
        }).fail(function (response) {
            $('#confirmEnable').modal('hide');
            runAlertError("Operation failed, please try again later");
            $('#Users').DataTable().ajax.reload();
        })
    });
}

let UserEdit = function () {
    var changeHome = $('#ChangeHome').val();
    var updateEmail = $('#UpdateEmail').val();

    $('#changeHome').on('click', function () {

        $('#changeHome').blur();
        $.ajax(changeHome, {
            type: 'POST',
            data: $('#editUser').serialize(),
            dataType: 'json',

        }).done(function (response) {

            if (response.success) {
                console.log(response);
                runAlertSuccess(response.message);
            } else {
                runAlertError(response.message);
            }
        }).fail(function (response) {
            runAlertError(response.message);
        })
    })

    $('#updateEmail').on('click', function () {

        $('#updateEmail').blur();
        $.ajax(updateEmail, {
            type: 'POST',
            data: $('#editUser').serialize(),
            dataType: 'json',

        }).done(function (response) {

            if (response.success) {
                runAlertSuccess(response.message);
            } else {
                runAlertError(response.message);
            }
        }).fail(function (response) {
            runAlertError(response.message);
        })
    })
}

let AddUser = function () {
    $(document).ready(function () {
        $('#RoleId').change(function () {

            var roleName = $('#RoleId option:selected').text();
            $('#RoleName').val(roleName);
            console.log($('#RoleName').val());
        })
    })

}

let CareHomeIndex = function () {
    $(document).ready(function () {

        var url = $('#GetCareHomes').val();
        var table = $('#Homes').DataTable({
            language: {
                "emptyTable": "No homes added yet"
            },
            pageLength: 10,
            pagingType: "numbers",
            ajax: {
                "url": url,
                "type": "GET",
                "dataType": "JSON"
            },
            "fnInitComplete": function (oSettings, json) {
                //console.log(json);
            },
            columns: [
                { data: 'Id', name: 'Id' },
                { data: 'HomeName', name: 'HomeName' },
                { data: 'PostCode', name: 'PostCode' },
                { data: 'Id', name: 'Id' },
            ],
            columnDefs: [
                { "className": "text-center", "targets": "_all" },
                {
                    targets: 0,
                    visible: false,
                },
                {
                    targets: 3,
                    render: function (a, b, data, d) {
                        var url = '';
                        if (data.Id != null) {
                            url = '<a class="btn btn-info btn-sm" href="CareHomeManagement/Edit/' + data.Id + '">Edit</a> <a class="btn btn-danger btn-sm" href="#" data-bs-toggle="modal" data-bs-target="#confirmDelete" data-id = "'+ data.Id +'">Delete</a>'
                        }
                        return url;
                    }
                }
            ]
        })
        var unitId = 0;
        $('#confirmDelete').on('show.bs.modal', function (e) {
            unitId = $(e.relatedTarget).data('id');
            console.log(unitId);
        })
        $('#confirmDelete').find('.modal-footer #confirm').on('click', function () {
            var urlToDelete = `CareHomeManagement/Delete/${unitId}`;
            console.log(urlToDelete);
            $.ajax(urlToDelete, {
                type: "POST",
                data: {
                    id: unitId,
                    __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
                }
            }).done(function (response) {
                console.log(response);
                $('#confirmDelete').modal('hide');
                if (response.success) {
                    runAlertSuccess("Operation successful")
                    $('#Homes').DataTable().ajax.reload();
                } else {
                    runAlertError("Operation failed, please try again later");
                }
            }).fail(function (response) {
                $('#confirmDelete').modal('hide');
                runAlertError("Operation failed, please try again later");
                $('#Homes').DataTable().ajax.reload();
            })
        });
    });
}

let QualificationIndex = function () {
    $(document).ready(function () {

        var url = $('#GetQualifications').val();
        var table = $('#Qualifications').DataTable({
            language: {
                "emptyTable": "No qualifications added yet"
            },
            pageLength: 10,
            pagingType: "numbers",
            ajax: {
                "url": url,
                "type": "GET",
                "dataType": "JSON"
            },
            "fnInitComplete": function (oSettings, json) {
                //console.log(json);
            },
            columns: [
                { data: 'Id', name: 'Id' },
                { data: 'Description', name: 'Description' },
                { data: 'DateCreated', name: 'DateCreated' },
                { data: 'Id', name: 'Id' },
            ],
            columnDefs: [
                { "className": "text-center", "targets": "_all" },
                {
                    targets: 0,
                    visible: false,
                },
                {
                    targets: 2,
                    render: function (a, b, data, d) {
                        if (data.DateCreated != null) {
                            moment.locale = "en-gb";
                            return moment(data.DateCreated).format("L")
                        }
                    }
                },
                {
                    targets: 3,
                    render: function (a, b, data, d) {
                        var url = '';
                        if (data.Id != null) {
                            url = '<a class="btn btn-info btn-sm" href="CareHomeManagement/Edit/' + data.Id + '">Edit</a> <a class="btn btn-danger btn-sm" href="#" data-bs-toggle="modal" data-bs-target="#confirmDelete" data-id = "' + data.Id + '">Delete</a>'
                        }
                        return url;
                    }
                }
            ]
        })
        var unitId = 0;
        $('#confirmDelete').on('show.bs.modal', function (e) {
            unitId = $(e.relatedTarget).data('id');
            console.log(unitId);
        })
        $('#confirmDelete').find('.modal-footer #confirm').on('click', function () {
            var urlToDelete = `Qualifications/Delete/${unitId}`;
            console.log(urlToDelete);
            $.ajax(urlToDelete, {
                type: "POST",
                data: {
                    id: unitId,
                    __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
                }
            }).done(function (response) {
                console.log(response);
                $('#confirmDelete').modal('hide');
                if (response.success) {
                    runAlertSuccess("Operation successful")
                    $('#Qualifications').DataTable().ajax.reload();
                } else {
                    runAlertError("Operation failed, please try again later");
                }
            }).fail(function (response) {
                $('#confirmDelete').modal('hide');
                runAlertError("Operation failed, please try again later");
                $('#Qualifications').DataTable().ajax.reload();
            })
        });
    });
}

let ManagerIndex = function () {
    $(document).ready(function () {

        var url = $('#GetAllStaff').val();
        var table = $('#Staffs').DataTable({
            language: {
                "emptyTable": "No staffs added yet"
            },
            pageLength: 10,
            pagingType: "numbers",
            ajax: {
                "url": url,
                "type": "GET",
                "dataType": "JSON"
            },
            "fnInitComplete": function (oSettings, json) {
                //console.log(json);
            },
            columns: [
                { data: 'FirstName', name: 'FirstName' },
                { data: 'LastName', name: 'LastName' },
                { data: 'JobTitle', name: 'JobTitle' },
                { data: 'DateCreated', name: 'DateCreated' },
                { data: 'Id', name: 'Id' },
            ],
            columnDefs: [
                { "className": "text-center", "targets": "_all" },
                {
                    targets: 3,
                    render: function (a, b, data, d) {
                        if (data.DateCreated != null) {
                            moment.locale = "en-gb";
                            return moment(data.DateCreated).format("L")
                        }
                    }
                },
                {
                    targets: 4,
                    render: function (a, b, data, d) {
                        var url = ``;
                        if (data.Id != null) {
                            url = `<a class="btn btn-info btn-sm" href="StaffManagement/Edit/` + data.Id + `">Edit</a> 
                            <a class="btn btn-danger btn-sm" href="#" data-bs-toggle="modal" data-bs-target="#confirmDelete" data-id = "` + data.Id + `">Delete</a>
                            </a> <a class="btn btn-info btn-sm" href="StaffQualifications/Index/` + data.Id + `">Manage qualifications</a>`
                        }
                        return url;
                    }
                }
            ]
        })
        $('#confirmDelete').on('show.bs.modal', function (e) {
            unitId = $(e.relatedTarget).data('id');
            console.log(unitId);
        })
        $('#confirmDelete').find('.modal-footer #confirm').on('click', function () {
            var urlToDelete = `/StaffManagement/Delete/${unitId}`;
            console.log(urlToDelete);
            $.ajax(urlToDelete, {
                type: "POST",
                data: {
                    id: unitId,
                    __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
                }
            }).done(function (response) {
                console.log(response);
                $('#confirmDelete').modal('hide');
                if (response.success) {
                    runAlertSuccess("Operation successful")
                    $('#Staffs').DataTable().ajax.reload();
                } else {
                    runAlertError("Operation failed, please try again later");
                }
            }).fail(function (response) {
                $('#confirmDelete').modal('hide');
                runAlertError("Operation failed, please try again later");
                $('#Staffs').DataTable().ajax.reload();
            })
        });
    });
}

let CreateQualification = function () {
    $(document).ready(function () {
        $(".Other").hide("hide")
        $('#QualificationId').on('change', function () {
            var QualId = $('#QualificationId option:selected').val();
            console.log(QualId);
            if (QualId == 6) {
                $(".Other").show("slow")
            } else if (QualId != 6) {
                $(".Other").hide("slow")
            }
        })

    });
}

let AddStaffQualification = function () {
    $(document).ready(function () {

        var url = $('#GetQualifications').val();
        var table = $('#StaffQualifications').DataTable({
            language: {
                "emptyTable": "No qualifications added yet"
            },
            pageLength: 10,
            pagingType: "numbers",
            ajax: {
                "url": url,
                "type": "GET",
                "dataType": "JSON"
            },
            "fnInitComplete": function (oSettings, json) {
                //console.log(json);
            },
            columns: [

                { data: 'Description', name: 'Description' },
                { data: 'AwardingOrganisation', name: 'AwardingOrganisation' },
                { data: 'DateAttainedTo', name: 'DateAttainedTo' },
                { data: 'Id', name: 'Id' }

            ],
            columnDefs: [
                { "className": "text-center", "targets": "_all" },
                {
                    targets: 2,
                    render: function (a, b, data, d) {
                        if (data.DateAttainedTo != null) {
                            moment.locale = "en-gb";
                            return moment(data.DateAttainedTo).format("L")
                        }
                    }
                },
                {
                    targets: 3,
                    render: function (a, b, data, d) {
                        var url = ``;
                        if (data.Id != null) {
                            url = `<a class="btn btn-info btn-sm" href="/StaffQualifications/Edit/` + data.Id + `">Edit</a> 
                            <a class="btn btn-danger btn-sm" href="#" data-bs-toggle="modal" data-bs-target="#confirmDelete" data-id = "` + data.Id + `">Delete</a>`
                        }
                        return url;
                    }
                }
            ]
        })

        $('#confirmDelete').on('show.bs.modal', function (e) {
            unitId = $(e.relatedTarget).data('id');
            console.log(unitId);
        })
        $('#confirmDelete').find('.modal-footer #confirm').on('click', function () {
            var urlToDelete = `/StaffQualifications/Delete/${unitId}`;
            console.log(urlToDelete);
            $.ajax(urlToDelete, {
                type: "POST",
                data: {
                    id: unitId,
                    __RequestVerificationToken: $("[name='__RequestVerificationToken']").val(),
                }
            }).done(function (response) {
                console.log(response);
                $('#confirmDelete').modal('hide');
                if (response.success) {
                    runAlertSuccess("Operation successful")
                    $('#StaffQualifications').DataTable().ajax.reload();
                } else {
                    runAlertError("Operation failed, please try again later");
                }
            }).fail(function (response) {
                $('#confirmDelete').modal('hide');
                runAlertError("Operation failed, please try again later");
                $('#StaffQualifications').DataTable().ajax.reload();
            })
        });

    });
}


































function runAlertSuccess(message) {
    if (typeof message === 'undefined') {
        message = 'Operation successful'
    }
    $.toast({
        heading: 'Success',
        text: message,
        showHideTransition: 'slide',
        icon: 'success',
        position: 'top-right'
    })
}
function runAlertError(message) {
    if (typeof message === 'undefined') {
        message = 'Operation failed.'
    }
    $.toast({
        heading: 'Error',
        text: message,
        showHideTransition: 'slide',
        icon: 'error',
        position: 'top-right'
    })
}
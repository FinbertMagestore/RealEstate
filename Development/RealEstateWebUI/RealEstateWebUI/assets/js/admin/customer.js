// index customer
//---------------
function changeSelectConditionFilterCustomer() {
    var ddlConditionFilter = document.getElementById("ddlConditionFilter").value;
    if (ddlConditionFilter == "TotalOrder") {
        $("#cfTotalOrder").removeClass("hide");
        $("#cfTotalCount").addClass("hide");
        $("#cfAcceptsMarketing").addClass("hide");
        $("#cfState").addClass("hide");
    } else if (ddlConditionFilter == "TotalCount") {
        $("#cfTotalOrder").addClass("hide");
        $("#cfTotalCount").removeClass("hide");
        $("#cfAcceptsMarketing").addClass("hide");
        $("#cfState").addClass("hide");
    } else if (ddlConditionFilter == "AcceptsMarketing") {
        $("#cfTotalOrder").addClass("hide");
        $("#cfTotalCount").addClass("hide");
        $("#cfAcceptsMarketing").removeClass("hide");
        $("#cfState").addClass("hide");
    } else if (ddlConditionFilter == "State") {
        $("#cfTotalOrder").addClass("hide");
        $("#cfTotalCount").addClass("hide");
        $("#cfAcceptsMarketing").addClass("hide");
        $("#cfState").removeClass("hide");
    } else {
        $("#cfTotalOrder").addClass("hide");
        $("#cfTotalCount").addClass("hide");
        $("#cfAcceptsMarketing").addClass("hide");
        $("#cfState").addClass("hide");
    }
}

// detail customer
//----------------
function displayShowMore() {
    $("#showMore").removeClass("hide");
    $("#displayShowMore").addClass("hide");
}

// edit customer info
function showModalEditCustomer(customerIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/editOverview/' + customerIDValue);
}

// delete customer
function showModalDeleteCustomer(customerIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/delete/' + customerIDValue);
}

function showModalAddAddressBook(customerIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/addAddressBook?customerID=' + customerIDValue);
}

// edit address book info
function showModalEditAddressBook(addressBookIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/editAddressBook/' + addressBookIDValue);
}

function showModalActiveAccount(customerIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/activeAccount/' + customerIDValue);
}

function showModalDisableAccount(customerIDValue) {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/admin/customers/disableAccount/' + customerIDValue);
}

//delete customer
//--------------
function submitDeleteCustomer(customerIDValue) {
    if (!$('#formDeleteCustomer').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/delete/" + customerIDValue,
        data: $('#formDeleteCustomer').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers?strMessage=delete1";
            }
        },
        error: function () {
        }
    });
}

// active account
//--------------
function submitActiveAccount(customerIDValue) {
    if (!$('#formActiveAccount').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/activeAccount/" + customerIDValue,
        data: $('#formActiveAccount').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerIDValue + "?strMessage=update1";
            } else if(data == 0) {
                alert("Kích hoạt tài khoản thất bại");
            }
        },
        error: function () {
        }
    });
}

// disable account
//---------------
function submitDisableAccount(customerIDValue) {
    if (!$('#formDisableAccount').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/disableAccount/" + customerIDValue,
        data: $('#formDisableAccount').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerIDValue + "?strMessage=update1";
            } else if (data == 0) {
                alert("Vô hiệu hoá tài khoản thất bại");
            }
        },
        error: function () {
        }
    });
}

// add addressbook
//---------------
function submitAddAddressBook(customerIDValue) {
    if (!$('#formAddAddressBook').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/addAddressBook?customerID=" + customerIDValue,
        data: $('#formAddAddressBook').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerIDValue + "?strMessage=update1";
            } else if (data == 0) {
                alert("Thêm sổ địa chỉ thất bại");
            }
        },
        error: function () {
        }
    });
}

// edit addressbook
//---------------
function submitEditAddressBook(addressBookIDValue, customerIDValue) {
    if (!$('#formEditAddressBook').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/editAddressBook/" + addressBookIDValue,
        data: $('#formEditAddressBook').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerIDValue + "?strMessage=update1";
            } else if (data == 0) {
                alert("Sửa sổ địa chỉ thất bại");
            }
        },
        error: function () {
        }
    });
}
function submitDeleteAddressBook(addressBookIDValue, customerIDValue) {
    if (!$('#formEditAddressBook').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/deleteAddressBook/" + addressBookIDValue,
        data: $('#formEditAddressBook').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerIDValue;
            }
        },
        error: function () {
        }
    });
}

// edit overview
//---------------
function submitEdit(customerID) {
    if (!$('#formEditOverview').valid()) return;
    return $.ajax({
        type: 'POST',
        url: "/admin/customers/editOverview/" + customerID,
        data: $('#formEditOverview').serialize(), //ID of your form
        async: true,
        success: function (data) {
            if (data == 1) {
                window.location.href = "/admin/customers/" + customerID+"?strMessage=update1";
            } else if (data == 0) {
                alert("Cập nhật thất bại");
            }
        },
        error: function () {
        }
    });
}

// dropdown customers
function viewCustomers() {
    return $.ajax({
        type: 'GET',
        url: "/admin/customers/dropdown",
        async: true,
        success: function (data) {
            document.getElementById("listCustomer").innerHTML = data;
        },
        error: function () {
        }
    });
}

function suggestCustomer() {
    clearTimeout(delayTimer);
    var delayTimer = setTimeout(function () {
        var temp = $("#ConditionFindCustomer").val();
        return $.ajax({
            type: 'GET',
            url: "/admin/customers/dropdown?" + "query=" + temp,
            async: true,
            success: function (data) {
                document.getElementById("listCustomer").innerHTML = "";
                document.getElementById("listCustomer").innerHTML = data;
            },
            error: function () {
            }
        });
    }, 300);
}

function addTag(tag) {
    var tagNew = $("input[name=Tags]").val();
    if (!tagNew.endsWith(',')) {
        tagNew += ",";
    }
    if (tagNew.startsWith(',')) {
        tagNew = tagNew.substring(1, tagNew.length);
    }
    tagNew += tag.innerHTML;
    $("input[name=Tags]").val(tagNew);
    $('input[name=Tags]').tagsinput('add', tag.innerHTML);
}
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
String.prototype.startsWith = function (needle) {
    return (this.indexOf(needle) == 0);
};
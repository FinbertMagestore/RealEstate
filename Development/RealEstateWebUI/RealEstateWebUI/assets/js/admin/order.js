var order = {
    Customer: {},
    BillingAddress: {},
    ShippingAddress: {},
    LineItems: [],
    OrderNote: "",
    BillingStatus: "",
    VariantChoiced: "",
};

// dropdown products
function viewLineItems() {
    return $.ajax({
        type: 'GET',
        url: "/admin/products/getLineItem?variantChoiced=" + order.VariantChoiced,
        async: true,
        success: function (data) {
            document.getElementById("lineItems").innerHTML = data;
        },
        error: function () {
        }
    });
}

function suggestLineItem() {
    clearTimeout(delayTimer);
    var delayTimer = setTimeout(function () {
        var temp = $("#ConditionFindLineItem").val();
        return $.ajax({
            type: 'GET',
            url: "/admin/products/getLineItem?variantChoiced=" + order.VariantChoiced + "&query=" + temp,
            async: true,
            success: function (data) {
                document.getElementById("lineItems").innerHTML = "";
                document.getElementById("lineItems").innerHTML = data;
            },
            error: function () {
            }
        });
    }, 300);
}

var lineItems = [];
var lineItemIndex = 0;
function choiceLineItem() {
    var formData = new FormData();
    var lineItemForm = $("#lineItemForm");
    if (!lineItemForm.valid()) return;
    var u = lineItemForm.serializeArray(), r;
    for (r = 0; r < u.length; ++r) {
        formData.append(u[r].name, u[r].value);
    }
    return $.ajax({
        type: 'POST',
        url: "/admin/products/getLineItem",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            $("#listLineItemDiv").removeClass("hide");
            $("#listLineItem").modal('hide');
            var strLineItem = '';
            if (data.length) {
                $.each(data, function (i, item) {
                    lineItems.push(item);
                    order.VariantChoiced += item.VariantID + ",";
                    strLineItem += templateLineItem(item, lineItemIndex);
                    lineItemIndex++;
                });
                $("#submitPaid").removeClass('disabled');
                $(".tableLineItem tbody").html($(".tableLineItem tbody").html() + strLineItem);
                updateTotalPrice();
            }
        },
        error: function () {
        }
    });
} suggestLineItem

function templateLineItem(lineItem, index) {
    var result = "";
    result += "<tr id='lineItem" + index + "'>";
    result += "<td>";
    if (lineItem.ImageUrl != null && lineItem.ImageUrl.trim() != '') {
        result += "<img src='" + lineItem.ImageUrl + "' alt='' width='50' height='50' />";
    } else {
        result += "<img class='product-thumb product-image-is-blank' width='50' height='50' src='/assets/images/no-image-50-50.png'>";
    }
    result += "</td>";
    if (lineItem.IsDefault) {
        result += "<td><a href='/admin/products/" + lineItem.ProductID + "'>" + lineItem.ProductName + "</a>";
    } else {
        result += "<td><a href='/admin/products/" + lineItem.ProductID + "/variants/" + lineItem.VariantID + "'>" + lineItem.ProductName + "</a>";
        result += "<p class='subdued'>" + lineItem.VariantName + "</p></td>";
    }
    result += "<td> <span class='line_item_price" + index + "'>" + lineItem.Price + "</span>₫</td>";
    result += "<td class='subdued'>x</td>";
    result += "<td><input class='form-control line_item_quantity" + index + "' name='quantity' type='number' value='" + lineItem.Quantity + "' onchange='changeQuantityOfOrder(this," + index + "," + lineItem.VariantID + ")'></td>";
    result += "<td class='text-center'> <span class='totalMoney line_item_totalMoney" + index + "'>" + (lineItem.Price * lineItem.Quantity) + "</span>₫</td>";
    result += "<td class='text-right'><a href='javascript:void(0)' onclick='removeLineItem(" + index + "," + lineItem.VariantID + ")'>X</a></td>";
    result += "</tr>";
    return result;
}

function removeLineItem(index, variantID) {
    var variantChoiced = order.VariantChoiced;
    $("#lineItem" + index).remove();
    variantChoiced = variantChoiced.replace(variantID + ",", "");
    if (lineItems && lineItems.length > 0) {
        for (var i = 0; i < lineItems.length; i++) {
            if (lineItems[i].VariantID == variantID) {
                lineItems.splice(i, 1);
            }
        }
    }
    updateTotalPrice();
    if (lineItems.length == 0) {
        $("#submitPaid").addClass('disabled');
    } else {
        $("#submitPaid").removeClass('disabled');
    }
}

function changeQuantityOfOrder(quantityOfLineItem, index, variantID) {
    var quantity = quantityOfLineItem.value;
    var price = $('.line_item_price' + index).text();
    $(".line_item_totalMoney" + index).text(quantity * price);
    updateTotalPrice();
    for (var i = 0; i < lineItems.length; i++) {
        if (lineItems[i].VariantID == variantID) {
            lineItems[i].Quantity = parseInt(quantity, 10);
        }
    }
}

function updateTotalPrice() {
    var totalMoneyElement = $(".totalMoney");
    var totalMoney = 0;
    for (var i = 0; i < totalMoneyElement.length; i++) {
        totalMoney += totalMoneyElement.eq(i).text() * 1;
    }
    $("#totalPrice").text(totalMoney);
    if (totalMoney == 0) {
        $("#submitPending").addClass('disabled');
    } else {
        $("#submitPending").removeClass('disabled');
    }
}

function selectCustomer(customerID) {
    return $.ajax({
        type: 'GET',
        url: "/admin/customers/getCustomer/" + customerID,
        async: true,
        dataType: 'json',
        processData: false,
        contentType: false,
        success: function (data) {
            order.Customer = data;
            order.BillingAddress.CustomerName = order.ShippingAddress.CustomerName = data.AddressBook.AddressBookFirstName + " " + data.AddressBook.AddressBookLastName;
            order.BillingAddress.HomeAddress = order.ShippingAddress.HomeAddress = data.AddressBook.HomeAddress;
            order.BillingAddress.ProvinceName = order.ShippingAddress.ProvinceName = data.AddressBook.ProvinceName;
            order.BillingAddress.CountryName = order.ShippingAddress.CountryName = data.AddressBook.Country.CountryName;
            order.BillingAddress.CountryID = order.ShippingAddress.CountryID = data.AddressBook.Country.CountryID;
            order.BillingAddress.Phone = order.ShippingAddress.Phone = data.AddressBook.Phone;

            $('#no_customer').addClass("hide");
            $('#customer').removeClass("hide");
            $("#choiceCustomer").modal('hide');

            $('#numOrder').text(data.TotalOrder);
            var customerName = data.CustomerFirstName + " " + data.CustomerLastName;
            $('#link2Customer').text(customerName);
            $('#link2Customer').attr('href', '/admin/customers/' + data.CustomerID);
            $('#emailCustomer').text(data.CustomerEmail).attr('href', 'mailto:' + data.CustomerEmail);
            $("#order_email").val(data.CustomerEmail);

            $('#shippingAddressName').text(data.AddressBook.AddressBookFirstName + " " + data.AddressBook.AddressBookLastName);
            $('#shippingAddressHomeAddress').text(data.AddressBook.HomeAddress);
            $('#shippingAddressProvince').text(data.AddressBook.ProvinceName);
            $('#shippingAddressCountry').text(data.AddressBook.Country.CountryName);

            $('#editShippingAddressName').val(data.AddressBook.AddressBookFirstName + " " + data.AddressBook.AddressBookLastName);
            $('#editShippingAddressPhone').val(data.AddressBook.Phone);
            $('#editShippingAddressHomeAddress').val(data.AddressBook.HomeAddress);
            $('#editShippingAddressProvinceName').val(data.AddressBook.ProvinceName);
            $('#ShippingAddress_CountryID').val(data.AddressBook.CountryID);

            $('#billingAddressName').text(data.AddressBook.AddressBookFirstName + " " + data.AddressBook.AddressBookLastName);
            $('#billingAddressAddress').text(data.AddressBook.HomeAddress);
            $('#billingAddressProvince').text(data.AddressBook.ProvinceName);
            $('#billingAddressCountry').text(data.AddressBook.Country.CountryName);

            $('#editBillingAddressName').val(data.AddressBook.AddressBookFirstName + " " + data.AddressBook.AddressBookLastName);
            $('#editBillingAddressPhone').val(data.AddressBook.Phone);
            $('#editBillingAddressHomeAddress').val(data.AddressBook.HomeAddress);
            $('#editBillingAddressProvinceName').val(data.AddressBook.ProvinceName);
            $('#BillingAddress_CountryID').val(data.AddressBook.CountryID);
        },
        error: function () {
        }
    });
}

function removeCustomer() {
    $('#no_customer').removeClass("hide");
    $("#customer").addClass("hide");
    order.Customer = {};
}

function isPaid() {
    order.BillingStatus = "paid";
    submitCreateOrder();
}

function isPending() {
    order.BillingStatus = "pending";
    submitCreateOrder();
}

function submitCreateOrder() {
    order.LineItems = lineItems;
    order.OrderNote = $('#OrderNote').val();

    return $.ajax({
        url: "/admin/orders/create",
        type: "POST",
        data: order,
        success: function (data) {
            if (!isNaN(data.id) && data.id > 0) {
                window.location.href = "/admin/orders/" + data.id;
            } else {
                document.getElementById("strMessage").innerHTML = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\">×</button>"+data.error+"</div>";
            }
        }
    });
}

function updateEmail() {
    $('#editEmail').modal('hide');
    var email = $("#order_email").val();
    order.Customer.CustomerEmail = email;
    $('#emailCustomer').text(email).attr('href', 'mailto:' + email);
}

function updateShippingAddress() {
    $('#editShippingAddress').modal('hide');
    order.ShippingAddress.CountryName = $("#ShippingAddress_CountryID :selected").text();
    order.ShippingAddress.CountryID = $("#ShippingAddress_CountryID").val();
    order.ShippingAddress.CustomerName = $('#editShippingAddressName').val();
    order.ShippingAddress.Phone = $('#editShippingAddressPhone').val();
    order.ShippingAddress.HomeAddress = $('#editShippingAddressHomeAddress').val();
    order.ShippingAddress.ProvinceName = $('#editShippingAddressProvinceName').val();

    $('#shippingAddressName').text(order.ShippingAddress.CustomerName);
    $('#shippingAddressHomeAddress').text(order.ShippingAddress.HomeAddress);
    $('#shippingAddressProvince').text(order.ShippingAddress.ProvinceName);
    $('#shippingAddressCountry').text(order.ShippingAddress.CountryName);
}

function updateBillingAddress() {
    $('#editBillingAddress').modal('hide');
    order.BillingAddress.CountryName = $("#BillingAddress_CountryID :selected").text();
    order.BillingAddress.CountryID = $("#BillingAddress_CountryID").val();
    order.BillingAddress.CustomerName = $('#editBillingAddressName').val();
    order.BillingAddress.Phone = $('#editBillingAddressPhone').val();
    order.BillingAddress.HomeAddress = $('#editBillingAddressHomeAddress').val();
    order.BillingAddress.ProvinceName = $('#editBillingAddressProvinceName').val();

    $('#billingAddressName').text(order.BillingAddress.CustomerName);
    $('#billingAddressAddress').text(order.BillingAddress.HomeAddress);
    $('#billingAddressProvince').text(order.BillingAddress.ProvinceName);
    $('#billingAddressCountry').text(order.BillingAddress.CountryName);
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

//----------
//detail
function submitMarkAsFulfilled(orderID) {
    return $.ajax({
        url: "/admin/orders/deliveryLineItem/" + orderID,
        type: "POST",
        success: function (data) {
            window.location.href = "/admin/orders/" + orderID + "?strMessage=" + data;
        }
    });
}

function submitMarkAsPaid(orderID) {
    return $.ajax({
        url: "/admin/orders/billingOrder/" + orderID,
        type: "POST",
        success: function (data) {
            window.location.href = "/admin/orders/" + orderID + "?strMessage=" + data;
        }
    });
}

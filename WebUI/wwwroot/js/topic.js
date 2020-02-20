function addDepartment() {
    if ($('#DepartmentName').val() === '') {
        toastr["error"]("Tên đơn vị không được để trống!");
    }
    else {
        $.ajax({
            url: '/admin/departments/quickcreate',
            type: 'POST',
            dataType: 'html',
            data: {
                name: $('#DepartmentName').val(),
                description: $('#DeparmentDescription').val()
            }
        }).done(function (response) {
            if (response > 0) {
                $('#DepartmentId').append($('<option>', {
                    value: response,
                    text: $('#DepartmentName').val()
                }));
                toastr["success"]("Thêm đơn vị thành công!");
                $('#DepartmentModal').modal('hide');
            }
            else {
                toastr["error"]("Thêm đơn vị thất bại!");
            }
        });
    }
}

function addCategory() {
    if ($('#CategoryName').val() === '') {
        toastr["error"]("Tên lĩnh vực không được để trống!");
    }
    else {
        $.ajax({
            url: '/admin/categories/quickcreate',
            type: 'POST',
            dataType: 'html',
            data: {
                name: $('#CategoryName').val(),
                description: $('#CategoryDescription').val()
            }
        }).done(function (response) {
            if (response > 0) {
                $('#CategoryId').append($('<option>', {
                    value: response,
                    text: $('#CategoryName').val()
                }));
                toastr["success"]("Thêm đơn vị thành công!");
                $('#CategoryModal').modal('hide');
            }
            else {
                toastr["error"]("Thêm đơn vị thất bại!");
            }
        });
    }
}
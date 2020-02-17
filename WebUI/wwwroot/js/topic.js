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
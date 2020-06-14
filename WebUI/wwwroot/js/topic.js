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
                $('#CategoryModal').modal('hide');
            }
        });
    }
}

function addCategory() {
    if ($('#CategoryName').val() === '') {
        toastr["error"]("Tên loại văn bản không được để trống!");
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
                toastr["success"]("Thêm loại văn bản thành công!");
                $('#CategoryModal').modal('hide');
            }
            else {
                toastr["error"]("Thêm loại văn bản thất bại!");
                $('#CategoryModal').modal('hide');
            }
        });
    }
}
function addTopicType() {
    if ($('#TopicTypeName').val() === '') {
        toastr["error"]("Tên loại tài liệu không được để trống!");
    }
    else {
        $.ajax({
            url: '/admin/topictypes/quickcreate',
            type: 'POST',
            dataType: 'html',
            data: {
                name: $('#TopicTypeName').val(),
                description: $('#TopicTypeDescription').val()
            }
        }).done(function (response) {
            if (response > 0) {
                $('#TopicTypeId').append($('<option>', {
                    value: response,
                    text: $('#TopicTypeName').val()
                }));
                toastr["success"]("Thêm loại tài liệu thành công!");
                $('#topic-type-modal').modal('hide');
            }
            else {
                toastr["error"]("Thêm loại tài liệu thất bại!");
                $('#topic-type-modal').modal('hide');
            }
        });
    }
}
function addAuthor() {
    if ($('#AuthorName').val() === '') {
        toastr["error"]("Tên nhà khoa học không được để trống!");
    }
    else {
        $.ajax({
            url: '/admin/authors/quickcreate',
            type: 'POST',
            dataType: 'html',
            data: {
                name: $('#AuthorName').val(),
                dateOfBirth: $('#AuthorDateOfBirth').val(),
                gender: $('#AuthorGender').val(),
                academicRank: $('#AuthorAcademicRank').val(),
                degree: $('#AuthorDegree').val(),
                deparment: $('#AuthorDeparment').val(),
                specialized: $('#AuthorSpecialized').val()
            }
        }).done(function (response) {
            if (response > 0) {
                $('#AuthorIds').append($('<option>', {
                    value: response,
                    text: $('#AuthorName').val()
                }));
                $('#AuthorModal').modal('hide');
                toastr["success"]("Thêm nhà khoa học thành công!");
            }
            else {
                $('#AuthorModal').modal('hide');
                toastr["error"]("Thêm nhà khoa học thất bại!");
            }
        });
    }
}
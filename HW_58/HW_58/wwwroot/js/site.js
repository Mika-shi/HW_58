// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#followButton').click(function () {
        let button = $(this);

        let userId = button.data('user-id');
        let isFollowing = button.data('is-following');

        let url;

        if (isFollowing) {
            url = '/Profile/Unfollow';
        } else {
            url = '/Profile/Follow';
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                id: userId
            },
            success: function (result) {
                if (result.success) {
                    $('#followersCount').text(result.followersCount);

                    if (isFollowing) {
                        button
                            .text('Follow')
                            .removeClass('btn-secondary')
                            .addClass('btn-primary');

                        button.data('is-following', false);
                    } else {
                        button
                            .text('Unfollow')
                            .removeClass('btn-primary')
                            .addClass('btn-secondary');

                        button.data('is-following', true);
                    }
                }
            }
        });
    });
});

$('#likeButton').click(function () {
    let button = $(this);

    let postId = button.data('post-id');
    let hasLiked = button.data('has-liked');

    let url;

    if (hasLiked) {
        url = '/Post/Unlike';
    } else {
        url = '/Post/Like';
    }

    $.ajax({
        url: url,
        type: 'POST',
        data: {
            id: postId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            if (result.success) {
                $('#likesCount').text(result.likesCount);

                if (hasLiked) {
                    button
                        .text('Like')
                        .removeClass('btn-secondary')
                        .addClass('btn-outline-primary');

                    button.data('has-liked', false);
                } else {
                    button
                        .text('Unlike')
                        .removeClass('btn-outline-primary')
                        .addClass('btn-secondary');

                    button.data('has-liked', true);
                }
            }
        }
    });
});

$(document).ready(function () {
    $('#followButton').click(function () {
        let button = $(this);

        let userId = button.data('user-id');
        let isFollowing = button.data('is-following');

        let url;

        if (isFollowing) {
            url = '/Profile/Unfollow';
        } else {
            url = '/Profile/Follow';
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: {
                id: userId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (result) {
                if (result.success) {
                    $('#followersCount').text(result.followersCount);

                    if (isFollowing) {
                        button
                            .text('Follow')
                            .removeClass('btn-secondary')
                            .addClass('btn-primary');

                        button.data('is-following', false);
                    } else {
                        button
                            .text('Unfollow')
                            .removeClass('btn-primary')
                            .addClass('btn-secondary');

                        button.data('is-following', true);
                    }
                }
            }
        });
    });
});

$('.delete-post-button').click(function () {
    let button = $(this);
    let postId = button.data('post-id');

    let confirmed = confirm('Are you sure you want to delete this post?');

    if (!confirmed) {
        return;
    }

    $.ajax({
        url: '/Post/DeleteAjax',
        type: 'POST',
        data: {
            id: postId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            if (result.success) {
                window.location.href = '/Profile/Details/' + result.userId;
            }
        },
        error: function () {
            alert('Failed to delete post.');
        }
    });
});

$('.edit-post-button').click(function () {
    let button = $(this);
    let postId = button.data('post-id');

    let description = $('#description-' + postId).val();

    $.ajax({
        url: '/Post/EditDescription',
        type: 'POST',
        data: {
            id: postId,
            description: description,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (result) {
            if (result.success) {
                $('#description-' + postId).val(result.description);
            }
        }
    });
});

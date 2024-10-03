document.getElementById('seeMoreBtn').addEventListener('click', function () {
    const hiddenContent = document.querySelector('.hidden-content');
    if (hiddenContent.style.display === 'none' || hiddenContent.style.display === '') {
        hiddenContent.style.display = 'block';
        this.textContent = 'Thu gọn';
    } else {
        hiddenContent.style.display = 'none';
        this.textContent = 'Xem thêm';
    }
});

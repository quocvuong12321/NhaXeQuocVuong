const backToTopButton = document.getElementById("backToTop");

// Hiện nút khi cuộn xuống
window.onscroll = function () {
    if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
        backToTopButton.style.display = "block"; // Hiện nút
    } else {
        backToTopButton.style.display = "none"; // Ẩn nút
    }
};

// Khi nhấp vào nút, quay lại đầu trang
backToTopButton.onclick = function () {
    window.scrollTo({
        top: 0,
        behavior: "smooth" // Hiệu ứng cuộn mượt mà
    });
};
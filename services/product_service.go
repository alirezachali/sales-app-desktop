package services

import (
	"sales-app-desktop/database"
	"sales-app-desktop/models"
)

// افزودن محصول جدید
func AddProduct(product *models.Product) error {
	return database.DB.Create(product).Error
}

// بدست آوردن تمام محصولات
func GetAllProducts() ([]models.Product, error) {
	var products []models.Product
	err := database.DB.Find(&products).Error
	return products, err
}

// جستجوی محصول با بارکد
func GetProductByBarcode(barcode string) (*models.Product, error) {
	var product models.Product
	err := database.DB.Where("barcode = ?", barcode).First(&product).Error
	return &product, err
}

// به‌روزرسانی محصول
func UpdateProduct(product *models.Product) error {
	return database.DB.Save(product).Error
}

// حذف محصول
func DeleteProduct(id uint) error {
	return database.DB.Delete(&models.Product{}, id).Error
}

// جستجوی محصول با نام
func SearchProduct(name string) ([]models.Product, error) {
	var products []models.Product
	err := database.DB.Where("name LIKE ?", "%"+name+"%").Find(&products).Error
	return products, err
}

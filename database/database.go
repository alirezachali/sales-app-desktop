package database

import (
	"sales-app-desktop/models"

	"gorm.io/driver/sqlite"
	"gorm.io/gorm"
)

var DB *gorm.DB

func InitDB() error {
	db, err := gorm.Open(sqlite.Open("sales.db"), &gorm.Config{})
	if err != nil {
		return err
	}
	DB = db

	// ایجاد جداول
	err = DB.AutoMigrate(&models.Product{}, &models.Category{})
	if err != nil {
		return err
	}

	return nil
}

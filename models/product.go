package models

import "gorm.io/gorm"

type Product struct {
	ID           uint    `gorm:"primaryKey"`
	Name         string  `gorm:"index"`
	Barcode      string  `gorm:"unique"`
	Price        float64
	Quantity     int
	ReorderLevel int
	Category     string
	Description  string
	CreatedAt    int64
	UpdatedAt    int64
}

type Category struct {
	ID   uint
	Name string `gorm:"unique"`
}

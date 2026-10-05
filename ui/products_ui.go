package ui

import (
	"sales-app-desktop/models"
	"sales-app-desktop/services"
	"strconv"

	"fyne.io/fyne/v2"
	"fyne.io/fyne/v2/container"
	"fyne.io/fyne/v2/data/binding"
	"fyne.io/fyne/v2/widget"
)

func ProductsTab() *fyne.Container {
	// واجد اطلاعات محصول
	nameEntry := widget.NewEntry()
	nameEntry.SetPlaceHolder("نام محصول")

	barcodeEntry := widget.NewEntry()
	barcodeEntry.SetPlaceHolder("بارکد")

	priceEntry := widget.NewEntry()
	priceEntry.SetPlaceHolder("قیمت")

	quantityEntry := widget.NewEntry()
	quantityEntry.SetPlaceHolder("موجودی")

	categoryEntry := widget.NewEntry()
	categoryEntry.SetPlaceHolder("دسته‌بندی")

	// لیست محصولات
	productList := widget.NewList(
		func() int {
			products, _ := services.GetAllProducts()
			return len(products)
		},
		func() fyne.CanvasObject {
			return widget.NewLabel("محصول")
		},
		func(id widget.ListItemID, obj fyne.CanvasObject) {
			products, _ := services.GetAllProducts()
			if id < len(products) {
				label := obj.(*widget.Label)
				p := products[id]
				label.SetText(p.Name + " - " + p.Barcode + " - " + strconv.FormatFloat(p.Price, 'f', 0, 64))
			}
		},
	)

	// دکمه افزودن
	addBtn := widget.NewButton("افزودن محصول", func() {
		price, _ := strconv.ParseFloat(priceEntry.Text, 64)
		quantity, _ := strconv.Atoi(quantityEntry.Text)

		product := &models.Product{
			Name:     nameEntry.Text,
			Barcode:  barcodeEntry.Text,
			Price:    price,
			Quantity: quantity,
			Category: categoryEntry.Text,
		}

		err := services.AddProduct(product)
		if err == nil {
			nameEntry.SetText("")
			barcodeEntry.SetText("")
			priceEntry.SetText("")
			quantityEntry.SetText("")
			categoryEntry.SetText("")
			productList.Refresh()
		}
	})

	// فرم
	form := container.NewVBox(
		widget.NewLabel("افزودن محصول جدید"),
		nameEntry,
		barcodeEntry,
		priceEntry,
		quantityEntry,
		categoryEntry,
		addBtn,
	)

	return container.NewVBox(
		form,
		widget.NewLabel("لیست محصولات"),
		productList,
	)
}

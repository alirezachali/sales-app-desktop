package main

import (
	"sales-app-desktop/database"
	"sales-app-desktop/ui"

	"fyne.io/fyne/v2/app"
	"fyne.io/fyne/v2/container"
	"fyne.io/fyne/v2/widget"
)

func main() {
	// اتصال دیتابیس
	err := database.InitDB()
	if err != nil {
		panic(err)
	}

	// ایجاد اپ
	myApp := app.New()
	myWindow := myApp.NewWindow()
	myWindow.SetTitle("سیستم فروش‌خانه")
	myWindow.Resize(fyne.NewSize(1000, 600))

	// تب‌ها
	tabs := container.NewAppTabs(
		container.NewTabItem("محصولات", ui.ProductsTab()),
		container.NewTabItem("فروش", widget.NewLabel("صفحه فروش")),
		container.NewTabItem("موجودی", widget.NewLabel("صفحه موجودی")),
	)

	myWindow.SetContent(tabs)
	myWindow.ShowAndRun()
}

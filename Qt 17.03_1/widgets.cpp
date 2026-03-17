#include "widgets.h"
#include "ui_widgets.h"

Widgets::Widgets(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    connect(ui->pushButton, &QPushButton::clicked, this, &Widgets::onStartClicked);
}

Widgets::~Widgets()
{
    delete ui;
}

void Widgets::onStartClicked()
{
    QString text = ui->lineEdit->text();
    ui->label->setText(text);
}
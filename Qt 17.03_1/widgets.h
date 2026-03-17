#ifndef WIDGETS_H
#define WIDGETS_H

#include <QMainWindow>

QT_BEGIN_NAMESPACE
namespace Ui {
class MainWindow;
}
QT_END_NAMESPACE

class Widgets : public QMainWindow
{
    Q_OBJECT

public:
    explicit Widgets(QWidget *parent = nullptr);
    ~Widgets();

private slots:
    void onStartClicked();

private:
    Ui::MainWindow *ui;
};

#endif
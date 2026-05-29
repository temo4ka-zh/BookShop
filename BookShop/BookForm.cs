using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BookShop
{
    public sealed class BookForm : Form
    {
        private readonly TextBox _titleTextBox;
        private readonly ComboBox _authorComboBox;
        private readonly ComboBox _genreComboBox;
        private readonly RadioButton _discountYesRadioButton;
        private readonly RadioButton _discountNoRadioButton;
        private readonly Book _sourceBook;

        public Book EditedBook { get; private set; }

        public BookForm(IEnumerable<Author> authors, IEnumerable<Genre> genres)
            : this(authors, genres, null)
        {
        }

        public BookForm(IEnumerable<Author> authors, IEnumerable<Genre> genres, Book book)
        {
            _titleTextBox = new TextBox();
            _authorComboBox = new ComboBox();
            _genreComboBox = new ComboBox();
            _discountYesRadioButton = new RadioButton();
            _discountNoRadioButton = new RadioButton();
            _sourceBook = book;

            InitializeComponent(authors.ToList(), genres.ToList());
            FillForm(book);
        }

        private void InitializeComponent(List<Author> authors, List<Genre> genres)
        {
            Text = _sourceBook == null ? "Добавление книги" : "Редактирование книги";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(470, 280);

            TableLayoutPanel rootPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(12)
            };
            rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _titleTextBox.Dock = DockStyle.Fill;

            _authorComboBox.Dock = DockStyle.Fill;
            _authorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _authorComboBox.DisplayMember = "Name";
            _authorComboBox.ValueMember = "Id";
            _authorComboBox.DataSource = authors;

            _genreComboBox.Dock = DockStyle.Fill;
            _genreComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _genreComboBox.DisplayMember = "Name";
            _genreComboBox.ValueMember = "Id";
            _genreComboBox.DataSource = genres;

            AddLabeledControl(rootPanel, 0, "Название:", _titleTextBox);
            AddLabeledControl(rootPanel, 1, "Автор:", _authorComboBox);
            AddLabeledControl(rootPanel, 2, "Жанр:", _genreComboBox);
            rootPanel.Controls.Add(new Label { Text = "Скидка:", AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 8, 8, 0) }, 0, 3);
            rootPanel.Controls.Add(CreateDiscountGroup(), 1, 3);

            Control buttonsPanel = CreateButtonsPanel();
            rootPanel.Controls.Add(buttonsPanel, 0, 4);
            rootPanel.SetColumnSpan(buttonsPanel, 2);

            Controls.Add(rootPanel);
        }

        private Control CreateDiscountGroup()
        {
            GroupBox groupBox = new GroupBox
            {
                Dock = DockStyle.Fill,
                Height = 70
            };

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            _discountYesRadioButton.Text = "Есть";
            _discountNoRadioButton.Text = "Нет";
            _discountNoRadioButton.Checked = true;

            panel.Controls.Add(_discountYesRadioButton);
            panel.Controls.Add(_discountNoRadioButton);
            groupBox.Controls.Add(panel);
            return groupBox;
        }

        private Control CreateButtonsPanel()
        {
            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 12, 0, 0)
            };

            Button cancelButton = new Button
            {
                Text = "Отмена",
                Width = 100,
                DialogResult = DialogResult.Cancel
            };

            Button okButton = new Button
            {
                Text = "OK",
                Width = 100
            };
            okButton.Click += delegate { ValidateAndSave(); };

            AcceptButton = okButton;
            CancelButton = cancelButton;

            panel.Controls.Add(cancelButton);
            panel.Controls.Add(okButton);
            return panel;
        }

        private static void AddLabeledControl(TableLayoutPanel panel, int row, string labelText, Control control)
        {
            control.Margin = new Padding(4, 6, 4, 6);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 8, 8, 0) }, 0, row);
            panel.Controls.Add(control, 1, row);
        }

        private void FillForm(Book book)
        {
            if (book == null)
            {
                return;
            }

            _titleTextBox.Text = book.Title;
            _authorComboBox.SelectedValue = book.AuthorId;
            _genreComboBox.SelectedValue = book.GenreId;
            _discountYesRadioButton.Checked = book.HasDiscount;
            _discountNoRadioButton.Checked = !book.HasDiscount;
        }

        private void ValidateAndSave()
        {
            if (string.IsNullOrWhiteSpace(_titleTextBox.Text))
            {
                MessageBox.Show("Введите название книги.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _titleTextBox.Focus();
                return;
            }

            Author author = _authorComboBox.SelectedItem as Author;
            if (author == null)
            {
                MessageBox.Show("Выберите автора.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Genre genre = _genreComboBox.SelectedItem as Genre;
            if (genre == null)
            {
                MessageBox.Show("Выберите жанр.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            EditedBook = new Book
            {
                Id = _sourceBook == null ? 0 : _sourceBook.Id,
                Title = _titleTextBox.Text.Trim(),
                AuthorId = author.Id,
                GenreId = genre.Id,
                HasDiscount = _discountYesRadioButton.Checked
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

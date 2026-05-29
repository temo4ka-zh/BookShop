using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BookShop
{
    public sealed class MainForm : Form
    {
        private readonly DataRepository _repository;
        private readonly DataGridView _booksGrid;
        private readonly ComboBox _authorsFilterComboBox;
        private readonly ComboBox _genresFilterComboBox;
        private readonly TextBox _titleTextBox;
        private readonly TextBox _authorTextBox;
        private readonly TextBox _genreTextBox;
        private readonly RadioButton _discountYesRadioButton;
        private readonly RadioButton _discountNoRadioButton;

        public MainForm()
        {
            _repository = new DataRepository();
            _booksGrid = new DataGridView();
            _authorsFilterComboBox = new ComboBox();
            _genresFilterComboBox = new ComboBox();
            _titleTextBox = new TextBox();
            _authorTextBox = new TextBox();
            _genreTextBox = new TextBox();
            _discountYesRadioButton = new RadioButton();
            _discountNoRadioButton = new RadioButton();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Книжная лавка";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(900, 550);
            Size = new Size(1100, 650);

            TableLayoutPanel rootPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            rootPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            MenuStrip menuStrip = CreateMenu();
            MainMenuStrip = menuStrip;
            rootPanel.Controls.Add(menuStrip, 0, 0);

            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill
            };
            rootPanel.Controls.Add(splitContainer, 0, 1);

            splitContainer.Panel1.Controls.Add(CreateBooksPanel());
            splitContainer.Panel2.Controls.Add(CreateBookDetailsPanel());

            Controls.Add(rootPanel);

            Load += MainForm_Load;
        }

        private MenuStrip CreateMenu()
        {
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem booksMenuItem = new ToolStripMenuItem("Книги");
            ToolStripMenuItem addMenuItem = new ToolStripMenuItem("Добавить", null, delegate { AddBook(); });
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Редактировать", null, delegate { EditBook(); });
            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Удалить", null, delegate { DeleteBook(); });
            ToolStripMenuItem reloadMenuItem = new ToolStripMenuItem("Перечитать из файлов", null, delegate { ReloadFromFiles(); });

            booksMenuItem.DropDownItems.Add(addMenuItem);
            booksMenuItem.DropDownItems.Add(editMenuItem);
            booksMenuItem.DropDownItems.Add(deleteMenuItem);
            booksMenuItem.DropDownItems.Add(new ToolStripSeparator());
            booksMenuItem.DropDownItems.Add(reloadMenuItem);

            menuStrip.Items.Add(booksMenuItem);
            return menuStrip;
        }

        private Control CreateBooksPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8)
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            FlowLayoutPanel filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false
            };

            _authorsFilterComboBox.Width = 220;
            _authorsFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _authorsFilterComboBox.SelectedIndexChanged += delegate { RefreshGrid(); };

            _genresFilterComboBox.Width = 220;
            _genresFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _genresFilterComboBox.SelectedIndexChanged += delegate { RefreshGrid(); };

            filterPanel.Controls.Add(new Label { Text = "Автор:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) });
            filterPanel.Controls.Add(_authorsFilterComboBox);
            filterPanel.Controls.Add(new Label { Text = "Жанр:", AutoSize = true, Padding = new Padding(14, 6, 0, 0) });
            filterPanel.Controls.Add(_genresFilterComboBox);

            ConfigureGrid();

            panel.Controls.Add(filterPanel, 0, 0);
            panel.Controls.Add(_booksGrid, 0, 1);
            return panel;
        }

        private Control CreateBookDetailsPanel()
        {
            GroupBox groupBox = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Карточка книги",
                Padding = new Padding(12)
            };

            TableLayoutPanel panel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 4
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            ConfigureReadOnlyTextBox(_titleTextBox);
            ConfigureReadOnlyTextBox(_authorTextBox);
            ConfigureReadOnlyTextBox(_genreTextBox);

            AddLabeledControl(panel, 0, "Название:", _titleTextBox);
            AddLabeledControl(panel, 1, "Автор:", _authorTextBox);
            AddLabeledControl(panel, 2, "Жанр:", _genreTextBox);

            GroupBox discountGroupBox = new GroupBox
            {
                Text = "Скидка на книгу",
                Dock = DockStyle.Fill,
                Height = 80
            };

            FlowLayoutPanel discountPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            _discountYesRadioButton.Text = "Есть";
            _discountYesRadioButton.Enabled = false;
            _discountNoRadioButton.Text = "Нет";
            _discountNoRadioButton.Enabled = false;

            discountPanel.Controls.Add(_discountYesRadioButton);
            discountPanel.Controls.Add(_discountNoRadioButton);
            discountGroupBox.Controls.Add(discountPanel);

            panel.Controls.Add(new Label { Text = "Наличие скидки:", AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 8, 8, 0) }, 0, 3);
            panel.Controls.Add(discountGroupBox, 1, 3);

            groupBox.Controls.Add(panel);
            return groupBox;
        }

        private void ConfigureGrid()
        {
            _booksGrid.Dock = DockStyle.Fill;
            _booksGrid.ReadOnly = true;
            _booksGrid.AllowUserToAddRows = false;
            _booksGrid.AllowUserToDeleteRows = false;
            _booksGrid.MultiSelect = false;
            _booksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _booksGrid.AutoGenerateColumns = false;
            _booksGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _booksGrid.SelectionChanged += delegate { UpdateSelectedBookInfo(); };
            _booksGrid.DoubleClick += delegate { EditBook(); };

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = "ID",
                FillWeight = 35
            });
            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Title",
                HeaderText = "Название"
            });
            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AuthorName",
                HeaderText = "Автор"
            });
            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GenreName",
                HeaderText = "Жанр"
            });
            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiscountText",
                HeaderText = "Скидка",
                FillWeight = 60
            });
        }

        private static void ConfigureReadOnlyTextBox(TextBox textBox)
        {
            textBox.Dock = DockStyle.Fill;
            textBox.ReadOnly = true;
        }

        private static void AddLabeledControl(TableLayoutPanel panel, int row, string labelText, Control control)
        {
            control.Margin = new Padding(4, 6, 4, 6);
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left, Padding = new Padding(0, 8, 8, 0) }, 0, row);
            panel.Controls.Add(control, 1, row);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ReloadFromFiles();
        }

        private void ReloadFromFiles()
        {
            try
            {
                _repository.Load();
                FillFilterComboBoxes();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить данные.\n\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillFilterComboBoxes()
        {
            List<Author> authors = new List<Author> { new Author { Id = 0, Name = "Все авторы" } };
            authors.AddRange(_repository.Authors);

            List<Genre> genres = new List<Genre> { new Genre { Id = 0, Name = "Все жанры" } };
            genres.AddRange(_repository.Genres);

            _authorsFilterComboBox.DisplayMember = "Name";
            _authorsFilterComboBox.ValueMember = "Id";
            _authorsFilterComboBox.DataSource = authors;

            _genresFilterComboBox.DisplayMember = "Name";
            _genresFilterComboBox.ValueMember = "Id";
            _genresFilterComboBox.DataSource = genres;
        }

        private void RefreshGrid()
        {
            RefreshGrid(0);
        }

        private void RefreshGrid(int selectedBookId)
        {
            int selectedAuthorId = GetSelectedAuthorId();
            int selectedGenreId = GetSelectedGenreId();

            List<BookGridRow> rows = _repository.Books
                .Where(book => selectedAuthorId == 0 || book.AuthorId == selectedAuthorId)
                .Where(book => selectedGenreId == 0 || book.GenreId == selectedGenreId)
                .OrderBy(book => book.Id)
                .Select(book => new BookGridRow
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorName = _repository.GetAuthorName(book.AuthorId),
                    GenreName = _repository.GetGenreName(book.GenreId),
                    DiscountText = book.HasDiscount ? "Есть" : "Нет"
                })
                .ToList();

            _booksGrid.DataSource = null;
            _booksGrid.DataSource = rows;

            if (rows.Count == 0)
            {
                ClearBookInfo();
                return;
            }

            int targetIndex = selectedBookId > 0 ? rows.FindIndex(row => row.Id == selectedBookId) : 0;
            if (targetIndex < 0)
            {
                targetIndex = 0;
            }

            _booksGrid.ClearSelection();
            _booksGrid.Rows[targetIndex].Selected = true;
            _booksGrid.CurrentCell = _booksGrid.Rows[targetIndex].Cells[0];
            UpdateSelectedBookInfo();
        }

        private int GetSelectedAuthorId()
        {
            Author author = _authorsFilterComboBox.SelectedItem as Author;
            return author == null ? 0 : author.Id;
        }

        private int GetSelectedGenreId()
        {
            Genre genre = _genresFilterComboBox.SelectedItem as Genre;
            return genre == null ? 0 : genre.Id;
        }

        private Book GetSelectedBook()
        {
            if (_booksGrid.CurrentRow == null)
            {
                return null;
            }

            BookGridRow selectedRow = _booksGrid.CurrentRow.DataBoundItem as BookGridRow;
            if (selectedRow == null)
            {
                return null;
            }

            return _repository.Books.FirstOrDefault(book => book.Id == selectedRow.Id);
        }

        private void UpdateSelectedBookInfo()
        {
            Book book = GetSelectedBook();
            if (book == null)
            {
                ClearBookInfo();
                return;
            }

            _titleTextBox.Text = book.Title;
            _authorTextBox.Text = _repository.GetAuthorName(book.AuthorId);
            _genreTextBox.Text = _repository.GetGenreName(book.GenreId);
            _discountYesRadioButton.Checked = book.HasDiscount;
            _discountNoRadioButton.Checked = !book.HasDiscount;
        }

        private void ClearBookInfo()
        {
            _titleTextBox.Clear();
            _authorTextBox.Clear();
            _genreTextBox.Clear();
            _discountYesRadioButton.Checked = false;
            _discountNoRadioButton.Checked = false;
        }

        private void AddBook()
        {
            using (BookForm form = new BookForm(_repository.Authors, _repository.Genres))
            {
                if (form.ShowDialog(this) != DialogResult.OK || form.EditedBook == null)
                {
                    return;
                }

                Book book = form.EditedBook;
                book.Id = _repository.Books.Count == 0 ? 1 : _repository.Books.Max(item => item.Id) + 1;
                _repository.Books.Add(book);

                _authorsFilterComboBox.SelectedValue = book.AuthorId;
                _genresFilterComboBox.SelectedValue = book.GenreId;
                RefreshGrid(book.Id);
            }
        }

        private void EditBook()
        {
            Book selectedBook = GetSelectedBook();
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для редактирования.", "Редактирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (BookForm form = new BookForm(_repository.Authors, _repository.Genres, selectedBook))
            {
                if (form.ShowDialog(this) != DialogResult.OK || form.EditedBook == null)
                {
                    return;
                }

                selectedBook.Title = form.EditedBook.Title;
                selectedBook.AuthorId = form.EditedBook.AuthorId;
                selectedBook.GenreId = form.EditedBook.GenreId;
                selectedBook.HasDiscount = form.EditedBook.HasDiscount;

                _authorsFilterComboBox.SelectedValue = selectedBook.AuthorId;
                _genresFilterComboBox.SelectedValue = selectedBook.GenreId;
                RefreshGrid(selectedBook.Id);
            }
        }

        private void DeleteBook()
        {
            Book selectedBook = GetSelectedBook();
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для удаления.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Удалить книгу '" + selectedBook.Title + "'?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _repository.Books.Remove(selectedBook);
            RefreshGrid();
        }
    }
}

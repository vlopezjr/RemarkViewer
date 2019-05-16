using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RemarkViewer
{
    public partial class RemarkViewer : UserControl
    {
        private MemoMeister.RemarkContext _context;
        private ToolTip _tooltip = new ToolTip();
        private string _contextId = "";
        private string _ownerId = "";

        public string UserId { get; set; } = "";

        public string ContextId
        {
            get { return _contextId; }
            set
            {
                _contextId = value;
                RefreshState();
            }
        }

        public string OwnerId
        {
            get { return _ownerId; }
            set
            {
                _ownerId = value;
                RefreshState();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text {
            get { return btnShow.Text; }
            set { btnShow.Text = value;  }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoMeister.RemarkContext Context {
            get { return _context; }
            set
            {
                _context = value;
                if (_context != null)
                {
                    ContextId = _context.ContextId;
                    OwnerId = _context.OwnerId;
                    UserId = _context.UserId;
                    RefreshState();
                }
            }
        }

        public RemarkViewer()
        {
            InitializeComponent();
            _context = new MemoMeister.RemarkContext();
        }

        public void AddRemark(ref object RemarkTypeIndex, string RemarkText)
        {
            _context.AddRemark(RemarkTypeIndex, RemarkText, Environment.UserName);
            UpdateControl(false);
        }

        public void Edit()
        {
            if (_context is null || ContextId.Length == 0 || OwnerId.Length == 0) return;
            _context.EditMemos();
            UpdateControl(false);
        }

        private void RefreshState()
        {
            if (ContextId.Length > 0 && OwnerId.Length > 0)
            {
                if (_context is null)
                {
                    _context = new MemoMeister.RemarkContext();
                    _context.Load(ContextId, OwnerId, UserId);
                }
                else if (ContextId != _context.ContextId)
                {
                    _context.Load(ContextId, OwnerId, UserId);
                }
                else if (OwnerId != _context.OwnerId)
                {
                    _context.OwnerId = OwnerId;
                }
                else
                    return;
            }
            else
            {
                btnShow.Image = imageList1.Images[0];
                btnShow.Text = "";
                _context = null;
            }

            if (_context != null)
                UpdateControl(true);
        }

        private void UpdateControl(bool DoPopups)
        {
            btnShow.Visible = true;
            btnShow.Enabled = true;

            if (_context.Remarks.Count <= 0)
            {
                btnShow.Image = imageList1.Images[0];
                _tooltip.SetToolTip(btnShow, "Click to add remarks for " + _context.OwnerName);
            }
            else
            {
                btnShow.Image = imageList1.Images[1];
                _tooltip.SetToolTip(btnShow, Summary());
            }

            if (DoPopups && _context.Remarks.Count > 0)
                _context.PopupMemos();
        }

        private string Summary()
        {
            string typeCaption = "";
            int remarkCount = 0;
            int typeCount = 0;
            string msg = "";

            foreach (MemoMeister.Remark remark in _context.Remarks)
            {
                if (!remark.Caption.Equals(typeCaption))
                {
                    if (remarkCount > 0)
                    {
                        if (msg.Length > 0)
                            msg += ", ";
                        msg += remarkCount + " " + typeCaption;
                        remarkCount = 0;
                    }
                    typeCaption = remark.Caption;
                    typeCount++;
                }
                remarkCount++;
            }

            if (remarkCount > 0)
            {
                if (msg.Length > 0)
                    msg += ", ";
                msg += remarkCount + " " + typeCaption;
            }

            return _context.Caption + ": " + msg;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (_context is null || ContextId.Length == 0 || OwnerId.Length == 0) return;
            _context.EditMemos();
            UpdateControl(false);
        }
         

        public void Access()
        {

        }
    }
}

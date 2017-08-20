namespace Tourmaline_Dashboard
{
    partial class membership_totals
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.membership_total_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.class_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.membership_total_chart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.class_chart)).BeginInit();
            this.SuspendLayout();
            // 
            // membership_total_chart
            // 
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.Name = "ChartArea1";
            this.membership_total_chart.ChartAreas.Add(chartArea1);
            this.membership_total_chart.Location = new System.Drawing.Point(12, 12);
            this.membership_total_chart.Name = "membership_total_chart";
            series1.BorderWidth = 5;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Series1";
            this.membership_total_chart.Series.Add(series1);
            this.membership_total_chart.Size = new System.Drawing.Size(568, 266);
            this.membership_total_chart.TabIndex = 5;
            title1.Name = "Title1";
            title1.Text = "Membership Over Time";
            this.membership_total_chart.Titles.Add(title1);
            this.membership_total_chart.Click += new System.EventHandler(this.membership_total_chart_Click);
            // 
            // class_chart
            // 
            chartArea2.Name = "main_chart_area";
            this.class_chart.ChartAreas.Add(chartArea2);
            this.class_chart.Location = new System.Drawing.Point(586, 12);
            this.class_chart.Name = "class_chart";
            series2.ChartArea = "main_chart_area";
            series2.Name = "Total Attendance";
            this.class_chart.Series.Add(series2);
            this.class_chart.Size = new System.Drawing.Size(435, 266);
            this.class_chart.TabIndex = 6;
            this.class_chart.Text = "chart1";
            title2.Name = "Title1";
            title2.Text = "Class Totals";
            this.class_chart.Titles.Add(title2);
            // 
            // membership_totals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 290);
            this.Controls.Add(this.class_chart);
            this.Controls.Add(this.membership_total_chart);
            this.Name = "membership_totals";
            this.Text = "Guild Membership";
            this.Load += new System.EventHandler(this.membership_totals_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.membership_total_chart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.class_chart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart membership_total_chart;
        private System.Windows.Forms.DataVisualization.Charting.Chart class_chart;
    }
}
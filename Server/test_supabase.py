import os
from supabase import create_client, Client

# さっきコピーしたURLとキーをここに貼る
url = "https://eldenejffhdnutjjakqh.supabase.co"
key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVsZGVuZWpmZmhkbnV0ampha3FoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzA4MzEyNjQsImV4cCI6MjA4NjQwNzI2NH0.DQmKqxZZnu5yg1Bc6HJ32fjxC9aYDxiLQRZlgM_hDZc"

# Supabaseに接続
supabase: Client = create_client(url, key)

# 送るデータのテスト（偽のゴーストデータ）
test_data = {
    "player_name": "PythonTestUser",
    "clear_time": 99.99,
    "motion_data": {"x": 1, "y": 2, "message": "Pythonからこんにちは"}
}

# データを送信（insert）
try:
    data = supabase.table("ghost_runs").insert(test_data).execute()
    print("成功しました！Supabaseを見てみてください。")
    print(data)
except Exception as e:
    print("エラーが出ました...")
    print(e)
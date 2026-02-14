from fastapi import FastAPI
from pydantic import BaseModel
from supabase import create_client, Client
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()

# ★Unity（ブラウザ）からの接続を許可する設定
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Supabaseの設定（自分のURLとキーに書き換える！）
url = "https://eldenejffhdnutjjakqh.supabase.co"
key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImVsZGVuZWpmZmhkbnV0ampha3FoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NzA4MzEyNjQsImV4cCI6MjA4NjQwNzI2NH0.DQmKqxZZnu5yg1Bc6HJ32fjxC9aYDxiLQRZlgM_hDZc"
supabase: Client = create_client(url, key)

# Unityから送られてくるデータの設計図
class GhostRequest(BaseModel):
    player_name: str
    clear_time: float
    motion_data: str
    secret_key: str
    recordInterval: float
    frames: list[dict]

@app.get("/")
def read_root():
    return {"message": "サーバーは正常に動いています！"}

@app.post("/upload")
def upload_ghost(data: GhostRequest):
    print(f"データ受信: {data.player_name} - {data.clear_time}秒")
    print(f"受け取った合言葉: {data.secret_key}")
    try:
        # Supabaseに保存
        res = supabase.table("ghost_runs").insert({
            "player_name": data.player_name,
            "clear_time": data.clear_time,
            "motion_data": data.motion_data,
            "secret_key": data.secret_key
        }).execute()
        return {"status": "success"}
    except Exception as e:
        print(f"エラー: {e}")
        return {"status": "error", "message": str(e)}
    
    # ... （今までのコードの下に追加） ...

# ★最新のゴーストを10件取得する機能
@app.get("/list")
def get_ghosts():
    try:
        # created_at（作った日）が新しい順に10件取得
        response = supabase.table("ghost_runs").select("*").order("created_at", desc=True).limit(10).execute()
        return {"status": "success", "ghosts": response.data}
    except Exception as e:
        return {"status": "error", "message": str(e)}
import random
import tkinter as tk
from itertools import cycle

# Constants
CANVAS_WIDTH, CANVAS_HEIGHT = 800, 400
EGG_WIDTH, EGG_HEIGHT, EGG_SCORE = 45, 55, 10
EGG_SPEED, EGG_INTERVAL, DIFFICULTY = 500, 4000, 0.95
CATCHER_WIDTH, CATCHER_HEIGHT = 100, 100

# Variables
score, lives_remaining = 0, 3
color_cycle = cycle(["light blue", "light green", "light pink", "light yellow", "light cyan"])
eggs = []

# Create Egg
def create_egg():
    x, y = random.randint(10, CANVAS_WIDTH - 10 - EGG_WIDTH), 40
    egg = canvas.create_oval(x, y, x + EGG_WIDTH, y + EGG_HEIGHT, fill=next(color_cycle))
    eggs.append(egg)
    root.after(EGG_INTERVAL, create_egg)

# Move Eggs
def move_eggs():
    for egg in eggs:
        coords = canvas.coords(egg)
        canvas.move(egg, 0, 10)
        if coords[3] > CANVAS_HEIGHT:
            egg_dropped(egg)
    root.after(EGG_SPEED, move_eggs)

# Egg Dropped
def egg_dropped(egg):
    eggs.remove(egg)
    canvas.delete(egg)
    lose_a_life()

# Lose a Life
def lose_a_life():
    global lives_remaining
    lives_remaining -= 1
    canvas.itemconfigure(lives_text, text="Lives: " + str(lives_remaining))
    if lives_remaining == 0:
        tk.messagebox.showinfo("Game Over!", "Final Score: " + str(score))
        root.destroy()

# Check Catch
def check_catch():
    catcher_coords = canvas.coords(catcher)
    for egg in eggs:
        egg_coords = canvas.coords(egg)
        if (catcher_coords[0] < egg_coords[0] and egg_coords[2] < catcher_coords[2] and
                catcher_coords[3] - egg_coords[3] < 40):
            eggs.remove(egg)
            canvas.delete(egg)
            increase_score(EGG_SCORE)
    root.after(100, check_catch)

# Increase Score
def increase_score(points):
    global score, EGG_SPEED, EGG_INTERVAL
    score += points
    EGG_SPEED = int(EGG_SPEED * DIFFICULTY)
    EGG_INTERVAL = int(EGG_INTERVAL * DIFFICULTY)
    canvas.itemconfigure(score_text, text="Score: " + str(score))

# Move Left
def move_left(event):
    x1, _, _, _ = canvas.coords(catcher)
    if x1 > 0:
        canvas.move(catcher, -20, 0)

# Move Right
def move_right(event):
    _, _, x2, _ = canvas.coords(catcher)
    if x2 < CANVAS_WIDTH:
        canvas.move(catcher, 20, 0)

# Initialize Game
root = tk.Tk()
root.title("Egg Catcher")
canvas = tk.Canvas(root, width=CANVAS_WIDTH, height=CANVAS_HEIGHT, background="deep sky blue")
canvas.create_rectangle(-5, CANVAS_HEIGHT - 100, CANVAS_WIDTH + 5, CANVAS_HEIGHT + 5, fill="sea green", width=0)
canvas.create_oval(-80, -80, 120, 120, fill='orange', width=0)
canvas.pack()

catcher = canvas.create_arc(CANVAS_WIDTH / 2 - CATCHER_WIDTH / 2, CANVAS_HEIGHT - CATCHER_HEIGHT - 20,
                            CANVAS_WIDTH / 2 + CATCHER_WIDTH / 2, CANVAS_HEIGHT - 20, start=200, extent=140,
                            style="arc", outline="blue", width=3)
game_font = tk.font.nametofont("TkFixedFont")
game_font.config(size=18)

score_text = canvas.create_text(10, 10, anchor="nw", font=game_font, fill="darkblue", text="Score: " + str(score))
lives_text = canvas.create_text(CANVAS_WIDTH - 10, 10, anchor="ne", font=game_font, fill="darkblue",
                               text="Lives: " + str(lives_remaining))

canvas.bind("<Left>", move_left)
canvas.bind("<Right>", move_right)
canvas.focus_set()

root.after(1000, create_egg)
root.after(1000, move_eggs)
root.after(1000, check_catch)

root.mainloop()
